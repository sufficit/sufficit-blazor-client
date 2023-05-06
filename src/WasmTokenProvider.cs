using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sufficit.Identity;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class WasmTokenProvider : ITokenProvider
    {
        private readonly IAccessTokenProvider _provider;

        public WasmTokenProvider(IAccessTokenProvider provider) 
        {
            _provider = provider;  
        }

        public async Task<string> GetTokenAsync()
        {
            var token = await _provider.RequestAccessToken();
            if (token.TryGetToken(out AccessToken accessToken))            
                return accessToken.Value;            

            throw new System.Exception("access token not available");
        }
    }
}
