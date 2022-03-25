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
using Sufficit.Blazor.Client.Models.Identity;

namespace Sufficit.Blazor.Client.Services
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
            _logger?.LogTrace($"instanced, storage user key: { STORAGEUSERKEY }");
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
            }
        }

        static IEnumerable<IDirective> Directives => Directive.Enumerator;

        static UserPolicy GetPolicy(string text)
        {
            var splitted = text.Split(':');
            var directive = Directives.FirstOrDefault(d => d.Key == splitted[0]);

            if (directive != null)
            {
                if (Guid.TryParse(splitted[1], out Guid IDContext))
                    return new UserPolicy(IDContext, directive);
            }

            throw new Exception($"could not create policy from values: { text }");
        }

        static Sufficit.Identity.UserPolicy GetPolicy(string id, string value)
        {
            var directive = Directives.FirstOrDefault(d => d.Key == id);
            if(directive != null)
            {
                if(Guid.TryParse(value, out Guid IDContext))
                    return new UserPolicy(IDContext, directive);
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
                        if (pair.Key == Sufficit.Identity.ClaimTypes.Directive)
                        {
                            if (pair.Value is System.Text.Json.JsonElement element)
                            {
                                switch (element.ValueKind)
                                {
                                    case JsonValueKind.String:
                                        {
                                            policies.Add(GetPolicy(element.GetString())); break;
                                        }
                                    case JsonValueKind.Array:
                                        foreach (var subelement in element.EnumerateArray())
                                            policies.Add(GetPolicy(subelement.GetString()));
                                        break;
                                    default: continue;
                                }
                                _logger?.LogDebug($"CreateUserAsync, directive: {element.ValueKind}");
                            }
                            account.AdditionalProperties.Remove(pair);
                        }
                    }
                }
            }
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomRemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {     
            CustomRemoteUserAccount userAccount = null;
            if (account != null) 
            {
                // ConvertUserPolicies(account);
                userAccount = await SetCachedUser(account); 
            }
            else userAccount = await GetCachedUser();
                       
            _logger?.LogDebug($"CreateUserAsync: { JsonSerializer.Serialize(userAccount?.AdditionalProperties) }");

            // as claims não são criadas por este método, crie após ...
            var user = await CreateUserAsyncBase(userAccount, options);
            if (!user.Identity.IsAuthenticated) return user;
             
            foreach (var claim in ToClaims(userAccount, options))
                ((ClaimsIdentity)user.Identity).AddClaim(claim);

            var currentOptions = new AuthenticationUserOptions();
            currentOptions.AuthenticationType = options.AuthenticationType;
            currentOptions.NameClaim = options.NameClaim;

            return new UserPrincipal(user, currentOptions);
        }

        public ValueTask<ClaimsPrincipal> CreateUserAsyncBase(CustomRemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            ClaimsIdentity claimsIdentity = (account != null) ? new ClaimsIdentity(options.AuthenticationType, options.NameClaim, options.RoleClaim) : new ClaimsIdentity();
            if (account != null)
            {
                foreach (KeyValuePair<string, object> additionalProperty in account.AdditionalProperties)
                {
                    string key = additionalProperty.Key;
                    if (additionalProperty.Value is JsonElement jsonElement)
                    {
                        if (jsonElement.ValueKind == JsonValueKind.Undefined || jsonElement.ValueKind == JsonValueKind.Null)
                        {
                            continue;
                        }                        

                        switch (jsonElement.ValueKind)
                        {
                            case JsonValueKind.String:
                                {
                                    claimsIdentity.AddClaim(new Claim(key, jsonElement.GetString()));
                                    break;
                                }
                            case JsonValueKind.Array:
                                foreach (var subelement in jsonElement.EnumerateArray())
                                    claimsIdentity.AddClaim(new Claim(key, subelement.ToString()));
                                break;
                            default: continue;
                        }
                    }
                }
            }

            return new ValueTask<ClaimsPrincipal>(new ClaimsPrincipal(claimsIdentity));
        }
    }
}
