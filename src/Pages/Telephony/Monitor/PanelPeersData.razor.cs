using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    public partial class PanelPeersData : ComponentBase, IDisposable
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
            if (int.TryParse(MaxSelected, out int size) && PageSize != size)
            {
                PageSize = size;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            EPService.Peers.OnChanged += Peers_OnChanged;            
        }

        private async void Peers_OnChanged(IMonitor? sender, object? state)
        {
            await Table.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            EPService.Peers.OnChanged -= Peers_OnChanged;
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
