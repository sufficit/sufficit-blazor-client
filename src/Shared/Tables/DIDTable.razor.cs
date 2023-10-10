using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Client;
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
        public uint Limit { get; set; } = 5;

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
        public Func<DIDSearchParameters, CancellationToken, ValueTask<IEnumerable<DirectInwardDialing>>>? GetData { get; set; } 

        [EditorRequired]
        protected MudTable<DirectInwardDialing>? Table { get; set; }

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

            if(Table != null)
                Table.Context.TableStateHasChanged = () => OnChanged.InvokeAsync();
        }

        protected async void FilterChanged(string text)
        {
            if (Parameters == null)
                Parameters = new DIDSearchParameters();

            Parameters.Extension = text;

            if (Table != null)            
                await Table.ReloadServerData();            
        }

        protected override void OnParametersSet()
        {
            if (Parameters != null)
            {
                if(Parameters.Extension != null)
                    Filter = Parameters.Extension;
            }
        }

        /// <summary>
        /// Reload server data
        /// </summary>
        public async Task DataBind()
        {
            if (Table != null)
            {
                await Table.ReloadServerData();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected TableData<DirectInwardDialing>? LastData;

        protected async Task<TableData<DirectInwardDialing>> InternalGetData(TableState _)
        {
            LastData = new TableData<DirectInwardDialing>();

            if (GetData == null)
                return LastData;

            // advertise that is loading
            await InvokeAsync(StateHasChanged);

            if (TokenSource != null)
                TokenSource.Cancel(false);
                        
            TokenSource = new CancellationTokenSource((int)TimeOut);
            try
            {
                var parameters = Parameters ?? new DIDSearchParameters();
                var response = await GetData(parameters, TokenSource.Token);
                LastData.Items = response ?? Array.Empty<DirectInwardDialing>();
            }
            catch (TaskCanceledException) { }
            return LastData;
        }
    }
}
