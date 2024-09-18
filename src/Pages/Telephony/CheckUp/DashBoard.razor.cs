using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.CheckUp
{
    [Authorize(Roles = ManagerRole.NormalizedName)]
    public partial class DashBoard : TelephonyBasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/checkup";

        public const string Title = "CheckUp";
        protected override string Description => "Conferência dos serviços de telefonia";

        #endregion

        [Inject]
        IWebSocketService WSClient { get; set; } = default!;

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
