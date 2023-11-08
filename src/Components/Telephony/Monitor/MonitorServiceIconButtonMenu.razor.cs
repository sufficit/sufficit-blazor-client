using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Sufficit.Telephony.EventsPanel;
using System;

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
        /*
        private async void OnControlChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }*/

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
    }
}
