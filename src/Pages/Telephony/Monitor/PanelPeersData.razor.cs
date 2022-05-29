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
    public partial class PanelPeersData 
    {
        [Inject]
        private EventsPanelService? Service { get; set; }

        protected Exception? ErrorConfig { get; set; }

        protected Table table { get; set; }

        protected Select SelectPageSize { get; set; }

        public string? FilterText { get; set; }

        public int PageSize { get; set; }

        public IList<PeerInfoMonitor> Items => Service?.Peers.ToList<PeerInfoMonitor>();

        public IEnumerable<PeerInfo> GetItems()
        {
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                string filter = FilterText.ToLowerInvariant().Trim();
                foreach (var item in Items.Where(s => s.Key.Contains(filter)).Take(PageSize))
                    yield return item;
            } else
            {
                foreach (var item in Items.Take(PageSize))
                    yield return item;
            }            
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Service != null)
            {
                try
                {
                    await Service.StartAsync(System.Threading.CancellationToken.None);
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }
            }
        }
        
        private void SelectPageSize_OnChanged(object sender, SelectedChangedEventArgs<string> e)
        {
            if(int.TryParse(e.Current, out int size) && PageSize != size)
            {
                PageSize = size;
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();            

            if (Service != null)
            {
                Service.Peers.OnChanged += Peers_OnChanged;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)            
                SelectPageSize.OnChanged += SelectPageSize_OnChanged;                
            
            if (int.TryParse(SelectPageSize.Selected, out int size) && PageSize != size)
            {
                PageSize = size;
                StateHasChanged();
            }
        }

        private async void Peers_OnChanged(IMonitor sender, object state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
