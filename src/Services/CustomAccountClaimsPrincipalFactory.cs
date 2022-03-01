using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using Sufficit;
using Sufficit.Identity;
using SufficitBlazorClient.Models.Identity;

namespace SufficitBlazorClient.Services
{    
    public class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<CustomRemoteUserAccount>
    {
        private const string STORAGEUSERKEY = "user";
        private readonly IAccessTokenProviderAccessor _accessor;
        private readonly ILogger _logger;
        private readonly ILocalStorageService _storage;

        public CustomAccountClaimsPrincipalFactory(
            ILogger<CustomAccountClaimsPrincipalFactory> logger,
            ILocalStorageService storage,
            IAccessTokenProviderAccessor accessor) : 
            base(accessor)
        {
            _accessor = accessor;
            _logger = logger;
            _storage = storage;
            _logger?.LogDebug($"instanced, storage user key: { STORAGEUSERKEY }");
        }

        public async Task<CustomRemoteUserAccount> GetCachedUser()
        {
            if (await _storage.ContainKeyAsync(STORAGEUSERKEY))
                return await _storage.GetItemAsync<CustomRemoteUserAccount>(STORAGEUSERKEY);
            else return null;
        }

        public async Task<CustomRemoteUserAccount> SetCachedUser(CustomRemoteUserAccount account)
        {            
            if(account == null)
                await _storage.RemoveItemAsync(STORAGEUSERKEY);
            else
                await _storage.SetItemAsync(STORAGEUSERKEY, account);    

            // não faz nenhum alteração, somente retorna para facilitar o próximo código 
            return account;
        }

        private IEnumerable<Claim> ToClaims(CustomRemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            if(account != null)
            {
                if(!string.IsNullOrWhiteSpace(account.Name))
                    yield return new Claim(options.NameClaim, account.Name);

                if (!string.IsNullOrWhiteSpace(account.Id))
                    yield return new Claim("sub", account.Id);

                if (account.Roles != null)
                {
                    foreach (var role in account.Roles)
                        yield return new Claim(options.RoleClaim, role);
                }
            }
        }

        static IEnumerable<IDirective> Directives => Utils.GetEnumerableOfType<IDirective>();

        static UserPolicy GetPolicy(string id, string value)
        {
            var directive = Directives.FirstOrDefault(d => d.ClaimType == id);
            if(directive != null)
            {
                if(Guid.TryParse(value, out Guid IDContext))
                    return new UserPolicy() { IDDirective = directive.ID, IDContext = IDContext };
            }

            throw new Exception($"could not create policy from values: { id } :: { value }");
        }

        public void ConvertUserPolicies(CustomRemoteUserAccount account)
        {
            if (account != null)
            {
                if (account.AdditionalProperties != null)
                {
                    var policies = new List<UserPolicy>();
                    foreach (var pair in account.AdditionalProperties)
                    {
                        if (pair.Key.StartsWith("directive_"))
                        {
                            if (pair.Value is System.Text.Json.JsonElement element)
                            {
                                switch (element.ValueKind)
                                {
                                    case JsonValueKind.String:
                                        {
                                            policies.Add(GetPolicy(pair.Key, element.GetString())); break;
                                        }
                                    case JsonValueKind.Array:
                                        foreach (var subelement in element.EnumerateArray())
                                            policies.Add(GetPolicy(pair.Key, subelement.GetString()));
                                        break;
                                    default: continue;
                                }
                                _logger?.LogDebug($"CreateUserAsync, directive: {element.ValueKind}");
                            }
                            account.AdditionalProperties.Remove(pair);
                        }
                    }
                    account.Policies = policies;
                }
            }
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomRemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {     
            CustomRemoteUserAccount userAccount = null;
            if (account != null) 
            {
                ConvertUserPolicies(account);
                userAccount = await SetCachedUser(account); 
            }
            else userAccount = await GetCachedUser();
                       
            _logger?.LogDebug($"CreateUserAsync: {JsonSerializer.Serialize(userAccount?.AdditionalProperties)}");

            // as claims não são criadas por este método, crie após ...
            var user = await base.CreateUserAsync(userAccount, options);
            if (!user.Identity.IsAuthenticated) return user;
             
            foreach (var claim in ToClaims(userAccount, options))
                ((ClaimsIdentity)user.Identity).AddClaim(claim);

            return user;
        }        
    }
}
