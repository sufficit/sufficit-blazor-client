using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Provisioning
{
    public partial class DashBoard : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "pages/provisioning/dashboard";
        protected override string? Area => "Provisionamento";

        public const string Title = "DashBoard";
        protected override string Description => "Provisioning Manager";

        #endregion

        [Parameter, SupplyParameterFromQuery(Name = "filter")]
        [EditorRequired]
        public string? FilterParameter { get; set; }

        [Parameter]
        public uint Limit { get; set; } = 25;

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        [Parameter]
        public uint PageSize { get; set; } = 25;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [EditorRequired]
        [CascadingParameter]
        protected UserPrincipal User { get; set; } = default!;

        [EditorRequired]
        protected MudTable<Sufficit.Telephony.Device> Table { get; set; } = default!;

        [EditorRequired]
        protected bool CanFilter => string.IsNullOrWhiteSpace(FilterParameter);

        /// <summary>
        ///     Internal filter text
        /// </summary>
        [EditorRequired]
        protected string? Filter { get; set; }

        protected IEnumerable<Sufficit.Telephony.Device> Items { get; set; } = Array.Empty<Sufficit.Telephony.Device>();

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(FilterParameter))
                Filter = FilterParameter;
        }

        CancellationTokenSource? TokenSource;
        protected async Task<TableData<Sufficit.Telephony.Device>> GetData(TableState _, CancellationToken cancellationToken)
        {            
            if (TokenSource != null)
                TokenSource.Cancel(false);

            var parameters = new DeviceSearchParameters();
            parameters.Limit = PageSize;
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                parameters.Order = new OrderBy() { Property = nameof(Sufficit.Telephony.Device.ContextId) };
                parameters.Filter = Filter;
            }
            else
            {
                parameters.Order = new OrderBy() { Property = nameof(Sufficit.Telephony.Device.Timestamp), Descending = true };
            }

            TokenSource = new CancellationTokenSource((int)TimeOut);
            Items = await APIClient.Provisioning.Search(parameters, TokenSource.Token);

            return new TableData<Sufficit.Telephony.Device>() { Items = this.Items };
        }

        protected async Task OnFilterChanged(string text)
        {
            Filter = text;

            if (Table != null)
                await Table.ReloadServerData();
        }
    }
}
