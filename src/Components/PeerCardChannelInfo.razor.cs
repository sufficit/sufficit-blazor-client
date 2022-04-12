using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class PeerCardChannelInfo
    {
        [Parameter]
        public ChannelInfoMonitor Channel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Channel.Changed += Channel_Changed;
        }

        private void Channel_Changed(object sender, AsteriskChannelState e)
        {
            StateHasChanged();
        }
    }
}
