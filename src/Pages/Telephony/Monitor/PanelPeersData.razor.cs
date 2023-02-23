using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    public partial class PanelPeersData : IDisposable
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

          protected Exception? ErrorConfig { get; set; }

        public string? FilterText { get; set; }

        public int PageSize { get; set; }

        public string? MaxSelected { get; internal set; }

        public IList<PeerInfoMonitor> Items => EPService.Peers.ToList<PeerInfoMonitor>();    

        public IEnumerable<PeerInfo> GetItems()
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
