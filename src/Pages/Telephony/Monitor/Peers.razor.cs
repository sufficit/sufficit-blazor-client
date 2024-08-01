using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = Sufficit.Identity.ManagerRole.NormalizedName)]
    public partial class Peers : MonitorTelephonyBasePageComponent, IDisposable, IPage
    {
        public const string RouteParameter = "pages/telephony/monitor/peers";

        public const string? Icon = MudBlazor.Icons.Material.Filled.SendTimeExtension;

        protected override string Title => "Pares";

        protected override string Description => "Estado de pares";

        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        protected Exception? ErrorConfig { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            EPService.Peers.OnChanged += Peers_OnChanged;
        }

        private async void Peers_OnChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            EPService.Peers.OnChanged -= Peers_OnChanged;
        }
    }
}
