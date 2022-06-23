using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Blazor.UI.Material;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ClientList : ComponentBase
    {
        [Inject]
        APIClientService APIClient { get; set; } = default!;

        [Parameter]
        public string? Filter { get; set; }

        [Parameter]
        public Action<Guid>? OnSelect { get; set; }

        [Parameter]
        public uint Limit { get; set; }

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        protected List<IClient> Clients { get; } = new List<IClient>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if(TextSearch != null)
                    TextSearch.OnValueChanged += TextSearch_OnValueChanged;

                await Update();
            }
        }

        private async void TextSearch_OnValueChanged(string? obj)
        {
            Filter = obj;
            await Update();
        }

        private async Task Update()
        {
            Clients.Clear();
            foreach (var item in await APIClient.Sales.GetClients(Filter, Limit))
            {
                Clients.Add(item);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}
