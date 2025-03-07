using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sufficit.Blazor.Services;
using Sufficit.Identity;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Services
{
    public class ClientAuthService : BlazorAuthService
    {
        public const string RouteParameter = "/authentication";

        private readonly NavigationManager _navigation;
        private readonly AuthenticationStateProvider _state;

        public ClientAuthService(NavigationManager navigation, AuthenticationStateProvider authenticationStateProvider)
        {
            _navigation = navigation;
            _state = authenticationStateProvider;
            _state.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        public override async ValueTask<UserPrincipal> CurrentUser()
        {
            var state = await _state.GetAuthenticationStateAsync();
            return state.User.ToUserPrincipal();
        }

        public override ValueTask Login (string? returnUrl)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["returnUrl"] = returnUrl?.Trim();

            _navigation.NavigateTo($"{RouteParameter}/login?{query}", forceLoad: true, replace: false);
            return ValueTask.CompletedTask;
        }

        public override ValueTask Logout(string? returnUrl)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["returnUrl"] = returnUrl?.Trim();

            _navigation.NavigateTo($"{RouteParameter}/logout?{query}", forceLoad: true, replace: false);
            return ValueTask.CompletedTask;
        }
    }
}
