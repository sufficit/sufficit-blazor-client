using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk.Events;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class PeerCard
    {
        [Parameter]
        public PeerMonitor Peer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Peer.Changed += Peer_Changed;
        }

        private void Peer_Changed(object sender, PeerStatusEnum e)
        {
            StateHasChanged();
        }
    }
}
