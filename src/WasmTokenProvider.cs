using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sufficit.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class WasmTokenProvider (IAccessTokenProvider provider, NavigationManager navigation) : ITokenProvider
    {
        public async ValueTask<string?> GetTokenAsync ()
        {
            var token = await provider.RequestAccessToken();
            if (token.TryGetToken(out AccessToken? accessToken))            
                return accessToken.Value;

            throw new AccessTokenNotAvailableException(navigation, token, null);
        }
    }
}
