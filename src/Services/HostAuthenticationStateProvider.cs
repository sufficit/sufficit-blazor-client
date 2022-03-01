using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Acesso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

        private const string LogInPath = "Account/Login";
        private const string LogOutPath = "Account/Logout";

        private readonly NavigationManager _navigation;
        private readonly HttpClient _client;
        private readonly ILogger<HostAuthenticationStateProvider> _logger;

        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<HostAuthenticationStateProvider> logger)
        {
            _navigation = navigation;
            _client = client;
            _logger = logger;
            _logger?.LogInformation("HostAuthenticationStateProvider instanced");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AuthenticationState loginuser = null;
            try
            {
                loginuser = new AuthenticationState(await GetUser(useCache: true));
            }
            catch
            {

            }
            return loginuser;
        }

        public void SignIn(string customReturnUrl = null)
        {
            _logger?.LogInformation("HostAuthenticationStateProvider SignIn");
            var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
            var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
            var logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
            _navigation.NavigateTo(logInUrl.ToString(), true);
        }

        public void SignOut()
        {
            _logger?.LogInformation("HostAuthenticationStateProvider SignOut");
            _navigation.NavigateTo(_navigation.ToAbsoluteUri(LogOutPath).ToString(), true);
        }

        private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
        {
            await Task.Yield();

            _logger?.LogInformation("HostAuthenticationStateProvider GetUser");
            var now = DateTimeOffset.Now;
            if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
            {
                _logger.LogDebug("Taking user from cache");
                return _cachedUser;
            }

            _logger.LogDebug("Fetching user");
            //_cachedUser = await FetchUser();
            _userLastCheck = now;

            return _cachedUser;
        }

        private async Task<ClaimsPrincipal> FetchUser()
        {
            _logger?.LogInformation("HostAuthenticationStateProvider FetchUser");
            UserInfo user = null;

            try
            {
                var response = await _client.GetAsync("User");
                response.EnsureSuccessStatusCode();

                user = await response.Content.ReadFromJsonAsync<UserInfo>();                
            }
            catch (Exception exc)
            {               
                _logger.LogError(exc, "Fetching user failed.");
            }

            if (user == null || !user.IsAuthenticated)
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            var identity = new ClaimsIdentity(
                nameof(HostAuthenticationStateProvider),
                user.NameClaimType,
                user.RoleClaimType);

            if (user.Claims != null)
            {
                foreach (var claim in user.Claims)
                {
                    identity.AddClaim(new Claim(claim.Type, claim.Value));
                }
            }

            return new ClaimsPrincipal(identity);
        }
    }
}
