using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ClientList : ComponentBase
    {
        [Inject]
        APIClientService APIClient { get; set; } = default!;
        
        protected bool Available => APIClient.Available;

        [Inject]
        IContextView ContextView { get; set; } = default!;

        [Parameter]
        public string? Filter { get; set; }

        private string? _filter = string.Empty;

        [EditorRequired]
        protected MudTable<IClient>? Table { get; set; } = default!;

        [Parameter]
        public EventCallback<string?> FilterChanged { get; set; }

        [Parameter]
        public EventCallback<IClient> SelectedItemChanged { get; set; }

        [Parameter]
        public uint Limit { get; set; }

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        protected bool Updating { get; set; }

        protected string RowClass(IClient client, int i)
        {
            return ContextView.ContextId == client.Id ? "bg-gradient-light" : string.Empty;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (_filter != Filter) {
                _filter = Filter;
                
                if (Table != null) 
                    await Table.ReloadServerData();

                await FilterChanged.InvokeAsync();
            }
        }

        protected IEnumerable<IClient> Clients { get; set; } = Array.Empty<IClient>();

        CancellationTokenSource? TokenSource;

        protected async Task<TableData<IClient>> GetData(TableState _, CancellationToken cancellationToken)
        {
            // only filter if text is set
            if (Available && !string.IsNullOrWhiteSpace(_filter))
            {
                if (TokenSource != null) 
                    TokenSource.Cancel(false);

                TokenSource = new CancellationTokenSource((int)TimeOut);
                Clients = await APIClient.Sales.GetClients(_filter, Limit, TokenSource.Token);
            } 
            else Clients = Array.Empty<IClient>();

            return new TableData<IClient>() { Items = Clients };
        }
    }
}
