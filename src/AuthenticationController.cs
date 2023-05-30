using AsterNET;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sufficit.Blazor.Client.Services;
using Sufficit.Blazor.Components;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    [Route($"{RouteParameter}/{{action}}")]
    public class AuthenticationController : ComponentBase
    {
        public const string RouteParameter = "clientauth";

        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        WasmAuthenticationService Service { get; set; } = default!;

        [Parameter]
        public string? Action { get; set; } = RemoteAuthenticationActions.Profile;

        protected override async Task OnParametersSetAsync()
        {
            switch (Action)
            {
                case RemoteAuthenticationActions.LogInCallback:
                    {
                        var result = await Service.CompleteSignInAsync(Navigation.Uri);

                        string url = result?.State?.ReturnUrl ?? "/";
                        Navigation.NavigateTo(url, false, true);
                        break;
                    }
                case RemoteAuthenticationActions.LogOutCallback:
                    {
                        var result = await Service.CompleteSignOutAsync(Navigation.Uri);

                        string url = result?.State?.ReturnUrl ?? "/";
                        Navigation.NavigateTo(url, false, true);
                        break;
                    }
                default: throw new NotImplementedException($"unknown action: {Action}");
            }
        }
    }
}
