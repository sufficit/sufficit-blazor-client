using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.CheckUp
{
    [Authorize(Roles = "manager")]
    public partial class DashBoard : TelephonyBasePageComponent
    {
        [Inject]
        IWebSocketService WSClient { get; set; } = default!;

        protected override string Title => "CheckUp";

        protected override string Description => "Conferência dos serviços de telefonia";

        protected override Task OnInitializedAsync()
        {
            WSClient.OnChanged += WSClient_OnChanged;
            WSClient.Test();

            return base.OnInitializedAsync();
        }

        private async void WSClient_OnChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
