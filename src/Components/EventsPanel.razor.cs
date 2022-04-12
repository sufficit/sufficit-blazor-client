using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class EventsPanel
    {
        [Inject]
        private EventsPanelService Service { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Service.OnChanged += Service_OnChanged;
            Service.Panel.Peers.CollectionChanged += Peers_CollectionChanged;
        }

        private void Service_OnChanged(object sender, System.EventArgs e)
        {
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await Service.StartAsync(System.Threading.CancellationToken.None);
            }
        }

        private void Peers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StateHasChanged();
        }

        protected IEnumerable<PeerMonitor> Peers 
            => Service.Panel.Peers
                .Where(filter => filter.Protocol != Asterisk.AsteriskChannelProtocol.LOCAL)
                .OrderByDescending(s => s.Channels.Count)
                .OrderBy(s => s.Id)
                .Take(30);
    }
}
