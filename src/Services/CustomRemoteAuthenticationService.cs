using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sufficit.Blazor.Client.Services
{
    public class CustomRemoteAuthenticationService : RemoteAuthenticationService<RemoteAuthenticationState, CustomRemoteUserAccount, OidcProviderOptions>
    {
        private readonly ILogger<CustomRemoteAuthenticationService> _logger;
        private readonly CustomAccountClaimsPrincipalFactory _factory;
        private readonly RemoteAuthenticationOptions<OidcProviderOptions> _options;
        private readonly IHttpContextAccessor _accessor;

        public CustomRemoteAuthenticationService(ILogger<CustomRemoteAuthenticationService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IJSRuntime jsRuntime, 
            IOptionsSnapshot<RemoteAuthenticationOptions<OidcProviderOptions>> options, 
            NavigationManager navigation,
            CustomAccountClaimsPrincipalFactory accountClaimsPrincipalFactory) : 
            base(jsRuntime, options, navigation, accountClaimsPrincipalFactory, logger)
        {
            _accessor = httpContextAccessor;
            _options = options.Value;
            _logger = logger;
            _factory = accountClaimsPrincipalFactory;
        }
        /*
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _logger?.LogTrace($"GetAuthenticationStateAsync ...");     
            var principal = await _factory.CreateUserAsync(null, _options.UserOptions);
            return new AuthenticationState(principal);            
        }

        public override async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            await _factory.SetCachedUser(null);
            return await base.SignOutAsync(context);
        }

        public override async ValueTask<AccessTokenResult> RequestAccessToken() => await RequestAccessToken(new AccessTokenRequestOptions());

        public override async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {            
            try
            {
                var user = await _factory.GetCachedUser();
                if (user != null && user.Token != null && user.Token.Expires > DateTime.Now)
                {
                    return new AccessTokenResult(AccessTokenResultStatus.Success, user.Token, options?.ReturnUrl);
                }

                _logger?.LogDebug($"Base Requesting AccessToken :: return => { options?.ReturnUrl }");

                var accessToken = await base.RequestAccessToken(options);
                _logger?.LogDebug($"Result access token status => { accessToken.Status }");

                if(accessToken.Status == AccessTokenResultStatus.Success)
                {
                    accessToken.TryGetToken(out AccessToken token);
                    if(token != null)
                    {
                        
                        if(user != null)
                        {
                            user.Token = token;
                            await _factory.SetCachedUser(user);
                        }
                    }
                }

                return accessToken;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                _logger.LogError($"AccessTokenNotAvailableException :: { exception.Message }");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        */
    }
}
