using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Charts;
using Newtonsoft.Json.Linq;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using Sufficit.Telephony;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Tables
{
    public partial class DIDTable : ComponentBase
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Parameter]
        public FilterUpdateParameters TimeOut { get; set; } = 1500;

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
        public string? Filter { get; set; }

        [EditorRequired]
        protected MudTable<DirectInwardDialing>? Table { get; set; } = default!;

        private CancellationTokenSource? TokenSource;

        protected IEnumerable<DirectInwardDialing> DataItems { get; set; } = Array.Empty<DirectInwardDialing>();

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contact.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
                return;

            DataBind();
        }

        /// <summary>
        /// Update filter parameter and reload server data
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetFilter(string? value)
        {
            if(Filter != value)
            {
                Filter = value;

                if (Table != null)
                {
                    await Table.ReloadServerData();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        /// <summary>
        /// Reload server data
        /// </summary>
        public async void DataBind()
        {
            if (Table != null)
            {
                await Table.ReloadServerData(); 
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task<TableData<DirectInwardDialing>> GetData(TableState _)
        {
            if (TokenSource != null)
                TokenSource.Cancel(false);
                        
            TokenSource = new CancellationTokenSource((int)TimeOut);
            try
            {
                var response = await APIClient.Telephony.DID.Filter(uid.Empty, TokenSource.Token);
                DataItems = response ?? Array.Empty<DirectInwardDialing>();
            }
            catch (TaskCanceledException) { DataItems = Array.Empty<DirectInwardDialing>(); }            

            return new TableData<DirectInwardDialing>() { Items = DataItems };
        }
    }
}
