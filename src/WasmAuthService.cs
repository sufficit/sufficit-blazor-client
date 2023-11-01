using Microsoft.AspNetCore.Components.Authorization;
using Sufficit.Blazor.Client.Services;
using Sufficit.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class WasmAuthService : IAuthService
    {
        private readonly WasmAuthenticationService _service;

        public WasmAuthService(WasmAuthenticationService service)
        {
            _service = service;
            _service.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if (AuthenticationStateChanged != null)
            {
                var state = await task;
                AuthenticationStateChanged.Invoke(state);
            }
        }

        public event Action<AuthenticationState>? AuthenticationStateChanged;

        public async ValueTask<UserPrincipal> CurrentUser()
        {
            var state = await _service.GetAuthenticationStateAsync();
            return (state.User as Sufficit.Identity.UserPrincipal)!;
        }

        public Task Login(string returnUrl)
            => _service.SignInAsync(returnUrl);

        public Task Logout()
            => _service.LogoutAsync();
    }
}
