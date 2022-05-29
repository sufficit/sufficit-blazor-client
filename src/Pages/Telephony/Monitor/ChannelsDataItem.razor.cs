using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material.Components;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    public partial class ChannelsDataItem
    {
        [Parameter]
        public ChannelInfoMonitor Item { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Item.OnChanged += Item_OnChanged;
        }

        private void Item_OnChanged(IMonitor sender, object state)
        {
            StateHasChanged();
        }
    }
}
