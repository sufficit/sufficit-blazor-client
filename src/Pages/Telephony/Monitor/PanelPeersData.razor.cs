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
    public partial class PanelPeersData : ComponentBase, IDisposable
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;
                
        protected MudTable<PeerInfoMonitor>? Table { get; set; }

        public string? FilterText { get; set; }

        public int PageSize { get; set; } = 10;

        public int TotalItems => EPService.Peers.Count;



        protected TableData<PeerInfoMonitor>? LastData;

        protected Task<TableData<PeerInfoMonitor>> GetData(TableState state, CancellationToken cancellationToken)
        {
            IEnumerable<PeerInfoMonitor> items = EPService.Peers.ToList();

            int totalfiltered, totalitems = items.Count();

            if (totalitems > 0)            
                items = items.Where(OnSearch);

            totalfiltered = items.Count();
            if (totalfiltered > 0)
            {
                if (Table?.Context != null)
                    items = Table.Context.Sort(items);

                if (state.PageSize > 0)
                {
                    var start = state.PageSize * state.Page;
                    var range = new Range(start, start + state.PageSize);
                    items = items.Take(range).ToList();
                }
            }

            LastData = new TableData<PeerInfoMonitor>() { Items = items, TotalItems = totalfiltered };
            return Task.FromResult(LastData);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            EPService.Peers.OnChanged -= OnPeersChanged;
            EPService.Peers.OnChanged += OnPeersChanged;
        }

        protected DateTime LastUpdate;
        protected CancellationTokenSource? CTSUpdate;

        private async void OnPeersChanged(IMonitor? sender, object? state)
        {
            if (Table != null)
            {
                if (CTSUpdate != null)
                    CTSUpdate.Cancel();

                CTSUpdate = new CancellationTokenSource();

                Task retrieveTask = Task.CompletedTask;
                if (LastUpdate.Add(TimeSpan.FromSeconds(1)) > DateTime.UtcNow)
                    retrieveTask = Task.Delay(2000, CTSUpdate.Token);

                await retrieveTask.ContinueWith(async previous =>
                {
                    if (previous.IsCompletedSuccessfully)
                    {
                        await Table.ReloadServerData();                        
                        LastUpdate = DateTime.UtcNow;
                    }
                });                
            }

            await InvokeAsync(StateHasChanged);
        }

        protected async Task OnRowsPerPageChanged(int rows)
        {
            PageSize = rows;
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            EPService.Peers.OnChanged -= OnPeersChanged;
        }

        public bool OnSearch(PeerInfoMonitor element)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;
            if (element.Content.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Content.Address?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false)
                return true;

            return false;
        }
    }
}
