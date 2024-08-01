using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Exchange
{
    [Authorize(Roles = Sufficit.Identity.ManagerRole.NormalizedName)]
    public partial class Messages : BasePageComponent, IPage
    {
        public const string RouteParameter = "pages/exchange/messages";

        public const string? Icon = MudBlazor.Icons.Material.Filled.Event;

        protected override string Title => "Mensagens";

        protected override string Description => "Registro de mensagens";

        protected override string? Area => "Exchange";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [SupplyParameterFromQuery]
        [Parameter]
        public string? ClassName { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public Guid? ReferenceId { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public Guid? ModelId { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public Guid? ContextId { get; set; }

        [Parameter]
        public MessageDetailsSearchParameters? Parameters { get; set; }

        [Parameter]
        public Func<MessageDetailsSearchParameters, CancellationToken, ValueTask<IEnumerable<MessageDetails>>>? GetData { get; set; }

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public EventCallback OnChanged { get; set; }

        protected MudTable<MessageDetails> Table { get; set; } = default!;

        public const string UNKNOWN = "*Desconhecido";
        protected async Task<string> GetTitle (Guid? id)
        {
            if (id.GetValueOrDefault() != Guid.Empty)
            {
                var contact = await APIClient.Contacts.GetContact(id!.Value, default);
                if (contact != null && !string.IsNullOrWhiteSpace(contact.Title)) 
                    return contact.Title;
            }

            return UNKNOWN;
        }

        public static string GetLink(Guid? contextid, Guid? referenceid, string? @class)
        {
            var link = RouteParameter + "?";
            if (contextid.HasValue)
                link += $"{nameof(ContextId)}={contextid}&";
            if (referenceid.HasValue)
                link += $"{nameof(ReferenceId)}={referenceid}&";
            if (!string.IsNullOrWhiteSpace(@class))
                link += $"{nameof(ClassName)}={@class}&";
            return link.TrimEnd('&');
        }

        protected override void OnParametersSet()
        {
            Parameters ??= new MessageDetailsSearchParameters();

            if (GetData == null)
                GetData = InternalGetData;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
                return;

            if (Table != null)
                Table.Context.TableStateHasChanged = async () => await OnChanged.InvokeAsync();
        }

        protected string? Filter { get; set; }

        protected string Totals
        {
            get
            {
                string result = string.Empty;                
                if (TableData.Items != null)
                {
                    int count = TableData.Items.Count();
                    result += count.ToString();
                    if (TableData.TotalItems > 0 && count != TableData.TotalItems)
                        result += " / " + TableData.TotalItems.ToString();
                }
                
                return result;
            }
        }

        protected async ValueTask<IEnumerable<MessageDetails>> InternalGetData (MessageDetailsSearchParameters parameters, CancellationToken cancellationToken)            
            => await APIClient.Exchange.Messages.GetMessages(parameters, cancellationToken);


        protected TableData<MessageDetails> TableData 
            = new TableData<MessageDetails>();

        protected async Task<TableData<MessageDetails>> TableServerData(TableState state, CancellationToken cancellationToken)
        {
            if (GetData == null)
                return TableData;

            try
            {
                Parameters ??= new MessageDetailsSearchParameters();

                var result = await GetData(Parameters, cancellationToken);
                TableData.Items = result;
                TableData.TotalItems = result.Count();
            }
            catch (OperationCanceledException) { }

            return TableData;
        }
    }
}
