using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "telephony")]
    public partial class Configuration : MonitorTelephonyBasePageComponent, IDisposable
    {
        public const string RouteParameter = "pages/telephony/monitor/configuration";

        public const string? Icon = Icons.Material.Filled.Settings;

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

        /// <summary>
        /// Last connection exception
        /// </summary>
        protected Exception? Exception => EPService.Exception;

        private IDisposable? DisposableForAMIOptions;

        protected override void OnParametersSet()
        {
            DisposableForAMIOptions = AMIOptionsMonitor.OnChange(AMIOptionsChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {            
            if (!firstRender) return;
            IsRendered = true;
            EPService.OnChanged += OnEPServiceChanged;
            UserOptions = await APIClient.Telephony.EventsPanel.GetUserOptions();
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            EPService.OnChanged -= OnEPServiceChanged;

            // releasing options monitor
            DisposableForAMIOptions?.Dispose();
        }

        protected async void OnStartClicked(MouseEventArgs _)
        {
            await EPService.StartAsync(System.Threading.CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(200);
            await InvokeAsync(StateHasChanged);
        }

        private async void OnEPServiceChanged(Microsoft.AspNetCore.SignalR.Client.HubConnectionState? _, Exception? __)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async void AMIOptionsChanged(AMIHubClientOptions? _, string? __)
        {
            if (IsRendered)
                await InvokeAsync(StateHasChanged);
        }

        protected async Task OnCheckBoxChanged(bool? value)
        {
            if (UserOptions != null)
            {
                if (UserOptions.OnlyPeers != value) 
                {
                    UserOptions.OnlyPeers = value;
                    await APIClient.Telephony.EventsPanel.PostUserOptions(UserOptions); 
                }
            }
        }
    }
}
