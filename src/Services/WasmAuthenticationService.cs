using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Blazored.LocalStorage;
using System.Threading;

namespace Sufficit.Blazor.Client.Services
{
    public class WasmAuthenticationService : RemoteAuthenticationService<RemoteAuthenticationState, RemoteUserAccount, OidcProviderOptions>
    {
        public WasmAuthenticationService(
            ILogger<WasmAuthenticationService> logger, 
            IJSRuntime jsRuntime, 
            IOptionsSnapshot<RemoteAuthenticationOptions<OidcProviderOptions>> options, 
            NavigationManager navigation,
            AccountClaimsPrincipalFactory<RemoteUserAccount> factory) : 
            base(jsRuntime, options, navigation, factory, logger) { }

        /// <summary>
        /// Converting to Sufficit Identity UserPrincipal
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask<ClaimsPrincipal> GetAuthenticatedUser()
        {
            return (await base.GetAuthenticatedUser()).ToUserPrincipal();
        }

        public Task SignInAsync(string returnUrl)
        {
            var context = new RemoteAuthenticationContext<RemoteAuthenticationState>();
            context.State = new RemoteAuthenticationState() { ReturnUrl = returnUrl };
            return base.SignInAsync(context);
        }

        /// <param name="responseUrl">url with all parameters from authentication service</param>
        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(string responseUrl)
        {
            var context = new RemoteAuthenticationContext<RemoteAuthenticationState>();
            context.Url = responseUrl;          
            return base.CompleteSignInAsync(context);
        }

        public Task LogoutAsync()
        {
            var context = new RemoteAuthenticationContext<RemoteAuthenticationState>();
            context.State = new RemoteAuthenticationState() { ReturnUrl = Navigation.Uri };
            return base.SignOutAsync(context);            
        }

        /// <param name="responseUrl">url with all parameters from authentication service</param>
        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(string responseUrl)      
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            var context = new RemoteAuthenticationContext<RemoteAuthenticationState>();
            context.Url = responseUrl;
            return base.CompleteSignOutAsync(context);            
        }
    }
}
