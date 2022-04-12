using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class PeerCardChannels
    {
        [Parameter]
        public ChannelInfoCollection Channels { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Channels.CollectionChanged += Channels_CollectionChanged;
        }

        private void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
