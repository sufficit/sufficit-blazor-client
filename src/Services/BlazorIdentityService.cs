using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sufficit.Blazor.UI.Material;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Services
{
    public class BlazorIdentityService
    {
        private readonly ILogger _logger;

        public BlazorIdentityService(IdentityClientService identity, APIClientService endpoints, ILogger<BlazorIdentityService> logger)
        {
            Identity = identity;
            Endpoints = endpoints;
            _logger = logger;   
        }

        public IdentityClientService Identity { get; }

        public APIClientService Endpoints { get; }

        public async Task<IEnumerable<IListItem>> GetDirectives(string filter, int results, CancellationToken cancellationToken)
        {
            var collection = new List<IListItem>();
            foreach (var item in await Endpoints.Identity.GetDirectives(filter, results, cancellationToken))
            {
                var listItem = new ListItem();
                listItem.Value = item.Key;
                listItem.Description = item.Name;
                listItem.Title = item.Description;
                listItem.Anchor = item.Key;
                collection.Add(listItem);
            }
            return collection;
        }

        public async Task<IContact> GetContact(Guid id, CancellationToken cancellationToken)
        {
            return await Endpoints.Contact.GetContact(id, cancellationToken);
        }

        public async Task<IEnumerable<IListItem>> GetContacts(string filter, int results, CancellationToken cancellationToken)
        {
            var collection = new List<IListItem>();
            collection.Add(new ListItem() { Value = Guid.Empty.ToString(), Description = "* Todos" });

            foreach (var item in await Endpoints.Contact.GetContacts(filter, results, cancellationToken))
            {
                var listItem = new ListItem();
                listItem.Value = item.ID.ToString();
                listItem.Description = item.Title;
                listItem.Title = item.Update.ToString();
                listItem.Anchor = item.Title;
                collection.Add(listItem);
            }
            return collection;
        }

        public async Task<IEnumerable<UserPolicyBase>> GetOldUserPolicies(User selected, CancellationToken cancellationToken = default)
        {
            var collection = new HashSet<UserPolicyBase>();
            await foreach (var userPolicy in GetOldUserPoliciesAsync(selected, cancellationToken))
                collection.Add(userPolicy);

            return collection;
        }

        public async IAsyncEnumerable<UserPolicyBase> GetOldUserPoliciesAsync(User selected, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var UserPoliciesBase = await Endpoints.Access.GetUserPolicies(selected.ID, cancellationToken);
            foreach (var basePolicy in UserPoliciesBase)
            {
                if (basePolicy.IDDirective == Guid.Empty)
                    throw new InvalidOperationException("empty directive id");
                else if (basePolicy.IDDirective == Guid.Parse("dc4065f6-ebc3-4a94-b1b4-03fd5641d7b5")) 
                    continue; // Ignoring obsolete directive : "visualizar atendimentos"
                else if (basePolicy.IDDirective == Guid.Parse("defa4589-96e7-4c9e-9207-5b60224f20f3")) 
                    continue; // Ignoring obsolete directive : "editar sacados"
                else if (basePolicy.IDDirective == Guid.Parse("e2094d6a-5434-4011-bbab-4654f7dd8d2c"))
                    continue; // Ignoring obsolete directive : "finalizar atendimento" 
                else if (basePolicy.IDDirective == Guid.Parse("f341290b-778c-44f1-bb50-e9ad646b08cd"))
                    continue; // Ignoring obsolete directive : "visualizar senhas"
                else if (basePolicy.IDDirective == Guid.Parse("4e4be1b2-20c9-4d80-9dd5-45b4447088f3"))
                    continue; // Ignoring obsolete directive : "susurrar em canais de audio"
                else if (basePolicy.IDDirective == Guid.Parse("46c1f014-3ca1-4371-a07c-092cf4b9baef"))
                    continue; // Ignoring obsolete directive : "alterar atendimento"
                else if (basePolicy.IDDirective == Guid.Parse("6c19ef98-0639-42c3-aee0-f0951a48b6b5"))
                    continue; // Ignoring obsolete directive : "alterar incidentes"
                else if (basePolicy.IDDirective == Guid.Parse("eab8782c-2e2e-4b96-88ce-ed61834050b0"))
                    continue; // Ignoring obsolete directive : "visualizar incidentes"

                yield return basePolicy;
            }            
        }

        public async Task<IEnumerable<UserClaimPolicy>> GetUserPolicies(User selected, CancellationToken cancellationToken = default)
        {
            var collection = new List<UserClaimPolicy>();
            var UserClaimsResponse = await Identity.Users.GetUserClaimsAsync(selected.ID, 0, 100, cancellationToken);
            if (UserClaimsResponse == null)            
                throw new System.Exception("null response");            

            if (UserClaimsResponse.Claims != null)
            {
                foreach (var claim in UserClaimsResponse.Claims)
                {
                    //_logger.LogInformation($"claim received: { claim.ClaimValue }");
                    if (claim.ClaimType == ClaimTypes.Directive)
                    {
                        var splitted = claim.ClaimValue.Split(":");
                        var policy = UserPolicy.Generate(splitted[0], splitted[1]);
                        var claimPolicy = new UserClaimPolicy(policy);
                        claimPolicy.UserClaimId = claim.Id;
                        collection.Add(claimPolicy);
                    }
                }
            }            

            return collection;
        }

        public async Task UpdateUserPolicy(User selected, string directive, Guid idcontext, CancellationToken cancellationToken = default)
        {
            var claim = new Sufficit.Identity.UserClaim();
            claim.UserId = selected.ID;
            claim.ClaimType = "directive";
            claim.ClaimValue = $"{ directive }:{ idcontext }";
            await Identity.Users.PostUserClaimsAsync(claim, cancellationToken);
        }

        public async Task UpdateUserPolicy(User selected, UserPolicyBase policy, CancellationToken cancellationToken = default)
        {
            var claim = new Sufficit.Identity.UserClaim();
            claim.UserId = selected.ID;
            claim.ClaimType = "directive";

            var directive = Directive.Enumerator.FirstOrDefault(s => s.ID == policy.IDDirective);
            if (directive == null)            
                throw new InvalidOperationException($"directive id not found: { policy.IDDirective }, for user: { selected.ID }");            

            claim.ClaimValue = $"{ directive.Key }:{ policy.IDContext }";
            await Identity.Users.PostUserClaimsAsync(claim, cancellationToken);
        }

        public async Task RemoveUserPolicy(User selected, int id, CancellationToken cancellationToken = default)
        {
            var claim = new Sufficit.Identity.UserClaim();
            claim.UserId = selected.ID;
            claim.Id = id;
            await Identity.Users.DeleteUserClaimsAsync(claim, cancellationToken);
        }

        public async Task SincronizeUserPolicies(User user, CancellationToken cancellationToken = default)
        {
            var taskOld = GetOldUserPolicies(user);
            var taskNew = GetUserPolicies(user);

            await Task.WhenAll(taskOld, taskNew);

            // removing that should not exists anymore
            foreach (var userPolicy in taskNew.Result.Except(taskOld.Result))
            {
                if (userPolicy is UserClaimPolicy claimPolicy)
                {
                    await RemoveUserPolicy(user, claimPolicy.UserClaimId.Value);
                }
            }

            foreach (var userPolicy in taskOld.Result.Except(taskNew.Result))
            {
                await UpdateUserPolicy(user, userPolicy);
            }
        }

        /// <summary>
        /// Reset user password and returns the new one
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> ResetPassword(Guid UserID, CancellationToken cancellationToken = default)
        {
            var request = new UserChangePasswordRequest();
            request.UserID = UserID;
            request.Password = request.ConfirmPassword = "!Abcd1234";
            await Identity.Users.ChangePasswordAsync(request, cancellationToken);
            return request.Password;
        }

        public async Task RemoveUser(User selected, CancellationToken cancellationToken = default) => 
            await Identity.Users.DeleteUserAsync(selected.ID, cancellationToken);
    }
}
