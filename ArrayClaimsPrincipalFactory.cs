using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace SufficitBlazorClient
{
    public class ArrayClaimsPrincipalFactory<TAccount> : AccountClaimsPrincipalFactory<TAccount> where TAccount:RemoteUserAccount
    {
        public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
            
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(TAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            return user;
            /*
            var initialUser = await base.CreateUserAsync(account, options);
            if (initialUser.Identity.IsAuthenticated)
            {
                foreach (var value in account.AuthenticationMethod)
                {
                    ((ClaimsIdentity)initialUser.Identity)
                        .AddClaim(new Claim("amr", value));
                }

                ((ClaimsIdentity)initialUser.Identity)
                        .AddClaim(new Claim("sub", account.ID.ToString()));

            }

            return initialUser;
            */
        }
    }
}
