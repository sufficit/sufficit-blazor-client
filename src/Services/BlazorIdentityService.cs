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
using System.Threading;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
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

        public async Task<IEnumerable<UserClaimPolicy>> GetUserPolicies(User selected, CancellationToken cancellationToken)
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
                        claimPolicy.UserClaimId = claim.ClaimID;
                        collection.Add(claimPolicy);
                    }
                }
            }            

            return collection;
        }

        public async Task UpdateUserPolicy(User selected, string directive, Guid idcontext, CancellationToken cancellationToken)
        {
            var claim = new UserClaim();
            claim.UserID = selected.ID;
            claim.ClaimType = "directive";
            claim.ClaimValue = $"{ directive }:{ idcontext }";
            await Identity.Users.PostUserClaimsAsync(claim, cancellationToken);
        }

        public async Task RemoveUserPolicy(User selected, int id, CancellationToken cancellationToken)
        {
            var claim = new UserClaim();
            claim.UserID = selected.ID;
            claim.ClaimID = id;
            await Identity.Users.DeleteUserClaimsAsync(claim, cancellationToken);
        }
    }
}
