using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Sufficit.Blazor.Components;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    public partial class PanelPeersData : BasePageComponent
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        protected Exception? ErrorConfig { get; set; }

        [EditorRequired]
        protected MudTable<PeerInfoMonitor> Table { get; set; } = default!;

        public string? FilterText { get; set; }

        public int PageSize { get; set; } = 10;

        public string? MaxSelected { get; internal set; }

        public int Counter => EPService.Peers.Count;

        protected Task<TableData<PeerInfoMonitor>> GetData(TableState _)
        {
            IEnumerable<PeerInfoMonitor> items = EPService.Peers;
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                string filter = FilterText.ToLowerInvariant().Trim();
                items = items.Where(s => s.Key.Contains(filter));
            }

            if (items.Any()) 
                items = items.Take(Table.RowsPerPage);

            var data = new TableData<PeerInfoMonitor>() { Items = items };
            return Task.FromResult(data);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            EPService.Peers.OnChanged += OnPeersChanged;

            if (int.TryParse(MaxSelected, out int size) && PageSize != size)
            {
                PageSize = size;
            }
        }

        private async void OnPeersChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose(bool disposing)
        {
            EPService.Peers.OnChanged -= OnPeersChanged;

            // following to base dispose
            base.Dispose(disposing);
        }

        public bool OnSearch(PeerInfoMonitor element)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;
            if (element.Content.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.Content.Time}".Contains(FilterText))
                return true;
            return false;

        }
    }
}
