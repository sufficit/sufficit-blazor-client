using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "manager")]
    public partial class ChannelsDataItem
    {
        [Parameter]
        [EditorRequired]
        public ChannelInfoMonitor Item { get; set; } = default!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();            
            Item.OnChanged += ItemChanged;
        }


        private async void ItemChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
