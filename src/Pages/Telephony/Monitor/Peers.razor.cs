using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "manager")]
    public partial class Peers : MonitorTelephonyBasePageComponent
    {
        protected override string Title => "Pares";

        protected override string Description => "Estado de pares";

        [Inject]
        private EventsPanelService? Service { get; set; }

        protected Exception? ErrorConfig { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Service != null)
            {
                try
                {
                    await Service.StartAsync(System.Threading.CancellationToken.None);
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(Service != null)
            {
                Service.Peers.OnChanged += Peers_OnChanged;
            }
        }

        private void Peers_OnChanged(IMonitor sender, object state)
        {
            
        }
    }
}
