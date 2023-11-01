using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "telephony")]
    public partial class Configuration : MonitorTelephonyBasePageComponent
    {
        public const string RouteParameter = "/pages/telephony/monitor/configuration";
        
        protected override string? Area => "Telefonia";

        protected override string Title => "Configurações";

        protected override string Description => "Opções do painel de eventos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IOptionsMonitor<AMIHubClientOptions> AMIOptionsMonitor { get; set; } = default!;

        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        protected EventsPanelUserOptions? UserOptions { get; set; }

        /// <summary>
        /// Indicates that it was rendered by first time to avoid javascript interop error
        /// </summary>
        protected bool IsRendered { get; set; }

        protected override void OnParametersSet()
        {
            AMIOptionsMonitor.OnChange(AMIOptionsChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {            
            if (!firstRender) return;
            IsRendered = true;
            EPService.OnChanged += OnEPServiceChanged;
            UserOptions = await APIClient.Telephony.EventsPanel.GetUserOptions();
            await InvokeAsync(StateHasChanged);
        }

        private async void OnEPServiceChanged(Microsoft.AspNetCore.SignalR.Client.HubConnectionState? arg1, Exception? arg2)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async void AMIOptionsChanged(AMIHubClientOptions? _, string? __)
        {
            if (IsRendered)
                await InvokeAsync(StateHasChanged);
        }

        protected async Task OnCheckBoxChanged(bool value)
        {
            if (UserOptions != null)
            {
                if (UserOptions.ShowTrunks != value) 
                {
                    UserOptions.ShowTrunks = value;
                    await APIClient.Telephony.EventsPanel.PostUserOptions(UserOptions); 
                }
            }
        }
    }
}
