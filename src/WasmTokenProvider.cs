using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sufficit.Identity;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class WasmTokenProvider (IAccessTokenProvider provider) : ITokenProvider
    {
        public async Task<string?> GetTokenAsync()
        {
            var token = await provider.RequestAccessToken();
            if (token.TryGetToken(out AccessToken? accessToken))            
                return accessToken.Value;            

            throw new System.Exception("access token not available");
        }
    }
}
