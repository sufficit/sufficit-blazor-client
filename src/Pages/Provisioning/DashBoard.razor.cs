using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Sufficit.Blazor.Client.Pages.Provisioning
{
    public partial class DashBoard : BasePageComponent
    {
        public const string RouteParameter = "pages/provisioning/dashboard";

        protected override string Title => "DashBoard";

        protected override string Description => "Provisioning Manager";

        [Parameter]
        public uint Limit { get; set; } = 25;

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        [Parameter]
        public uint PageSize { get; set; } = 25;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private ExceptionControlService Exceptions { get; set; } = default!;

        [EditorRequired]
        [CascadingParameter]
        protected UserPrincipal User { get; set; } = default!;

        [EditorRequired]
        protected MudTable<Sufficit.Telephony.Device>? Table { get; set; } = default!;

        [EditorRequired]
        protected string FilterText { get; set; } = string.Empty;

        protected IEnumerable<Sufficit.Telephony.Device> Items { get; set; } = Array.Empty<Sufficit.Telephony.Device>();


        CancellationTokenSource? TokenSource;
        protected async Task<TableData<Sufficit.Telephony.Device>> GetData(TableState _)
        {            
            if (TokenSource != null)
                TokenSource.Cancel(false);

            var parameters = new DeviceSearchParameters();
            if (!string.IsNullOrWhiteSpace(FilterText)) 
                parameters.Filter = FilterText;
            parameters.Limit = PageSize;

            TokenSource = new CancellationTokenSource((int)TimeOut);
            Items = await APIClient.Provisioning.Search(parameters, TokenSource.Token);            

            return new TableData<Sufficit.Telephony.Device>() { Items = Items.OrderBy(s => s.ContextId) };
        }

        protected async Task OnFilterChanged(string text)
        {
            FilterText = text;

            if (Table != null)
                await Table.ReloadServerData();
        }

        private bool FilterFunc(Sufficit.Telephony.Device element)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;

            if (Guid.TryParse(FilterText, out var id))
            {
                if (element.ExtensionId == id || element.ContextId == id)
                    return true;
            }
            else
            {
                if (element.MACAddress.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    return true;
              
                if (element.IPAddress != null && element.IPAddress.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }


    }
}
