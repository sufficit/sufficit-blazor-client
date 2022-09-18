using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.UI.Material;
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

        [Inject]
        IContextView View { get; set; } = default!;

        [Parameter]
        public string? Filter {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Table?.ReloadServerData();
                }
            }
        }

        private string? _filter = string.Empty;

        [EditorRequired]
        protected MudTable<IClient>? Table { get; set; }

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
            return View.ContextId == client.Id ? "bg-gradient-light" : string.Empty;
        }

        protected IEnumerable<IClient> Clients { get; set; } = Array.Empty<IClient>();

        CancellationTokenSource? TokenSource;

        protected async Task<TableData<IClient>> GetData(TableState _)
        {
            // only filter if text is set
            if (!string.IsNullOrWhiteSpace(_filter))
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
