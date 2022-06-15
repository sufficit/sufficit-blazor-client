using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    [Authorize(Roles = "manager")]
    public partial class CheckUp : TelephonyBasePageComponent
    {
        [Inject]
        IWebSocketService WSClient { get; set; }

        protected override string Title => "CheckUp";

        protected override string Description => "Conferência dos serviços de telefonia";

        protected override Task OnInitializedAsync()
        {
            WSClient.OnChanged += WSClient_OnChanged;
            WSClient.Test();

            return base.OnInitializedAsync();
        }

        private void WSClient_OnChanged(object sender, EventArgs e)
        {
            this.StateHasChanged();
        }
    }
}
