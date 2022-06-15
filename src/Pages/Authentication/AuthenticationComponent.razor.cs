using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Sufficit.Blazor.Client.Pages.Authentication
{
    public partial class AuthenticationComponent : ComponentBase
    {
        [Inject]
        BlazorAuthenticationStateProvider AuthenticationState { get; set; } = default!;

        [Inject]
        IAuthService AuthService { get; set; } = default!;

        [Parameter] 
        public string? Action { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        public string? UserName { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var state = await AuthenticationState.GetAuthenticationStateAsync();
                UserName = state.User.Identity?.Name;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            switch (Action)
            {
                case "login":
                    await AuthService.Login(ReturnUrl ?? "/"); break;

                case "logout":
                    await AuthService.Logout(); break;

                default: break;
            }

        }
    }
}
