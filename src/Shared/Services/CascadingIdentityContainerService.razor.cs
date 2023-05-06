using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Services
{
    public partial class CascadingIdentityContainerService : ComponentBase, IDisposable
    {
        [Inject]
        IAuthService Authentication { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected UserPrincipal User { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            User = await Authentication.CurrentUser();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            Authentication.AuthenticationStateChanged += AuthStateChanged;
        }

        public void Dispose()
        {
            Authentication.AuthenticationStateChanged -= AuthStateChanged;
        }

        protected async void AuthStateChanged(AuthenticationState state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}