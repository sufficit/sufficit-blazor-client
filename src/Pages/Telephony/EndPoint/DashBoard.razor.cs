using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.EndPoint
{
    [Authorize(Roles = "telephony")]
    public partial class DashBoard : TelephonyBasePageComponent, IDisposable
    {
        public const string RouteParameter = "pages/telephony/endpoint/dashboard";

        public const string? Icon = Icons.Material.Filled.Extension;

        protected override string Title => "Extensões";

        protected override string Description => "Ramais | Dispositivos | EndPoints";

        public uint PageSize { get; set; } = 25;

        [EditorRequired]
        protected MudTable<Sufficit.Telephony.EndPoint> Table { get; set; } = default!;

        protected IEnumerable<Sufficit.Telephony.EndPoint> DataItems { get; set; } = Array.Empty<Sufficit.Telephony.EndPoint>();

        [Parameter]
        public uint Limit { get; set; } = 20;

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public uint TimeOut { get; set; } = 10000;

        [Parameter]
        public string? Filter { get; set; }

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        public EventCallback<Sufficit.Telephony.EndPoint> SelectedItemChanged { get; set; }

        protected string? GetInfoLink(Guid id)
            => $"/?contactid={id:N}";
                        
        private async void ContextViewChanged(Guid? _)
        {
            await Task.Delay(100); 
            await InvokeAsync(StateHasChanged);

            DataBind();
        }

        protected void OnTextChanged(string? value)
        {
            if (Filter != value)
            {
                Filter = value;

                // only filter if text is set
                if (string.IsNullOrWhiteSpace(Filter) || Filter.Trim().Length >= Minimum)                
                    DataBind();                
            }
        }

        /// <summary>
        /// Reload server data
        /// </summary>
        public async void DataBind()
        {
            if (ContextView.ContextId.GetValueOrDefault() == Guid.Empty)
                return;

            if (Table != null)            
                await Table.ReloadServerData();            
        }

        /// <summary>
        ///     Getting data cancellation token source
        /// </summary>
        private CancellationTokenSource? TokenSource;

        protected async Task<TableData<Sufficit.Telephony.EndPoint>> GetData(TableState _)
        {           
            if (TokenSource != null)
                TokenSource.Cancel(false);

            var parameters = new EndPointSearchParameters()
            {
                ContextId = ContextView.ContextId,
                Filter = Filter?.Trim().ToLowerInvariant(),
                Limit = Limit,
            };
              
            TokenSource = new CancellationTokenSource((int)TimeOut);
            try
            {
                DataItems = await APIClient.Telephony.EndPoint.GetEndPoints(parameters, TokenSource.Token);
                DataItems = DataItems.OrderBy(x => x.Title);

                await InvokeAsync(StateHasChanged);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }

            return new TableData<Sufficit.Telephony.EndPoint>() { Items = DataItems };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            _ = await ContextView.Default();

            // if change, get items again
            ContextView.OnChanged += ContextViewChanged;

            // getting items for the first time
            DataBind();
        }

        protected bool IsMailBox(Sufficit.Telephony.EndPoint? source)
            => source != null && !source.TechIAX && !source.TechPJSIP && !source.TechSIP;
        

        public void Dispose()
        {
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
