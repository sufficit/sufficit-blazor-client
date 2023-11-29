using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Client;
using Sufficit.EndPoints;
using Sufficit.Telephony;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Tables
{
    public partial class DIDTable : ComponentBase
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Parameter]
        public DIDSearchParameters? Parameters { get; set; }

        [Parameter]
        public uint? Limit { get; set; }

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        [Parameter]
        public EventCallback OnChanged { get; set; }

        [Parameter]
        public EventCallback<Guid> OnContextSelected { get; set; }

        [Parameter]
        public Func<DIDSearchParameters, CancellationToken, ValueTask<EndPointFullResponse<DirectInwardDialing>>>? GetData { get; set; }

        [EditorRequired]
        protected MudTable<DirectInwardDialing> Table { get; set; } = default!;

        protected bool HasContextId => Parameters?.ContextId.HasValue ?? false;

        protected string? Filter { get; set; }

        protected string Totals
        {
            get
            {
                string result = string.Empty;
                if (LastData != null) {
                    if (LastData.Items != null) {
                        int count = LastData.Items.Count();
                        result += count.ToString();
                        if (LastData.TotalItems > 0 && count != LastData.TotalItems)
                            result += " / " + LastData.TotalItems.ToString();
                    }
                }
                return result;
            }
        }

        private CancellationTokenSource? TokenSource;

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contacts.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
                return;

            if (Table != null)
                Table.Context.TableStateHasChanged = async () => await OnChanged.InvokeAsync();
        }

        protected CancellationTokenSource? FilterCancellationTokenSource;

        protected async void FilterChanged(string? text)
        {
            var normalized = text?.Trim().TrimStart('+');
            if (Filter != normalized)
            {
                Filter = normalized;

                if (string.IsNullOrWhiteSpace(Filter) || Filter.Length >= Minimum)
                {
                    Parameters ??= new DIDSearchParameters();
                    Parameters.Extension = Filter;

                    if (Table != null)
                    {
                        if (FilterCancellationTokenSource != null)
                            FilterCancellationTokenSource.Cancel();

                        var cancel = FilterCancellationTokenSource = new CancellationTokenSource();

                        // delaying for avoid unecessary requests
                        await Task.Delay(500);

                        if (!cancel.IsCancellationRequested)
                            await Table.ReloadServerData();
                    }
                }
            }
        }

        protected override void OnParametersSet()
        {
            Parameters ??= new DIDSearchParameters();
            Filter = Parameters.Extension?.Text;            

            if (GetData == null)
                GetData = InternalGetData;
        }

        protected async ValueTask<EndPointFullResponse<DirectInwardDialing>> InternalGetData (DIDSearchParameters parameters, CancellationToken cancellationToken)
            => await APIClient.Telephony.DID.FullSearch(parameters, cancellationToken); 
        
        /// <summary>
        /// Reload server data
        /// </summary>
        public async ValueTask DataBind()
        {
            if (Table != null)            
                await Table.ReloadServerData();
        }

        protected TableData<DirectInwardDialing>? LastData;

        protected async Task<TableData<DirectInwardDialing>> TableServerData(TableState state)
        {            
            LastData ??= new TableData<DirectInwardDialing>()
            {
                Items = Array.Empty<DirectInwardDialing>()
            };

            if (GetData == null)
                return LastData;

            if (TokenSource != null)
                TokenSource.Cancel(false);

            var ts = TokenSource = new CancellationTokenSource((int)TimeOut);
            try
            {
                Parameters ??= new DIDSearchParameters();
                if (Parameters.Limit == null && Limit == null)
                    Parameters.Limit = 20;

                var result = await GetData(Parameters, ts.Token);
                LastData.Items = result.Data;
                LastData.TotalItems = (int)result.Total;
            }
            catch (OperationCanceledException) { }
            return LastData;
        }

        protected async Task ContextSelect(Guid contextid)
        {
            if (Parameters != null)
            {
                if (Parameters.ContextId != contextid)                
                    Parameters.ContextId = contextid;                
            }

            if (OnContextSelected.HasDelegate)            
                await OnContextSelected.InvokeAsync(contextid);            
        }
    }
}
