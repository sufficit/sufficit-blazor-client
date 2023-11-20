using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Telephony.Monitor
{
    public partial class MonitorServiceIconButtonMenu : ComponentBase, IDisposable
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            EPService.OnChanged += OnControlChanged;
        }

        private async void OnControlChanged(HubConnectionState? state, Exception? ex)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async void OnStartClicked(MouseEventArgs _)
        {
            await EPService.StartAsync(System.Threading.CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(200);
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            EPService.OnChanged -= OnControlChanged;
        }

        protected MudBlazor.Color Color
        {
            get
            {
                if (!EPService.IsConnected) return Color.Dark;
                if (!EPService.IsTrying) return Color.Info;
                return Color.Primary;
            }
        }

        protected string? Animation
            => EPService.IsTrying ? "glow" : null;
    }
}
