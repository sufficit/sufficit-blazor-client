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
    public partial class PanelPeersData : IDisposable
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

          protected Exception? ErrorConfig { get; set; }

        [EditorRequired]
        protected MudTable<PeerInfo>? Table { get; set; } = default!;

        public string? FilterText { get; set; }

        public int PageSize { get; set; } = 10;

        public string? MaxSelected { get; internal set; }

        public int Counter => EPService.Peers.Count;

        public IList<PeerInfoMonitor> Items => EPService.Peers.ToList();    

        public IEnumerable<Sufficit.Telephony.EventsPanel.PeerInfo> GetItems()
        {
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                string filter = FilterText.ToLowerInvariant().Trim();
                foreach (var item in Items.Where(s => s.Key.Contains(filter)).Take(PageSize))
                    yield return item;
            } else {
                foreach (var item in Items.Take(PageSize))
                    yield return item;
            }            
        }

        protected async Task<TableData<Sufficit.Telephony.EventsPanel.PeerInfo>> GetData(TableState _)
        {
            await Task.Yield();
            return new TableData<Sufficit.Telephony.EventsPanel.PeerInfo>() { Items = GetItems() };
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
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            EPService.Peers.OnChanged -= Peers_OnChanged;
        }

        public bool OnSearch(PeerInfo element)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;
            if (element.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.Time}".Contains(FilterText))
                return true;
            return false;

        }
    }
}
