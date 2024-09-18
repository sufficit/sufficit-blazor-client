using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class Calls : TelephonyBasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/calls";

        public const string Title = "Chamadas";
        protected override string Description => "Chamadas Telefônicas";

        #endregion

        [Inject]
        APIClientService Service { get; set; } = default!;

        [Inject]
        IContextView ContextView { get; set; } = default!;


        protected IEnumerable<ICallRecordBasic>? CallList;

        private string? Error;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            ContextView.OnChanged += ContextChanged;

            var contextid = ContextView.ContextId.GetValueOrDefault();
            if (contextid != Guid.Empty)
                await Filter(contextid);
        }

        protected async void ContextChanged(Guid? value)
            => await Filter(value.GetValueOrDefault());

        protected async Task Filter(Guid ContextId)
        {
            CallList = null;
            Error = string.Empty;

            if (ContextId != Guid.Empty)
            {
                var parameters = new CallSearchParameters();
                parameters.ContextId = ContextId;
                parameters.Start = DateTime.Now.Date;

                try
                {
                    CallList = await Service.Telephony.CallSearchAsync(parameters, CancellationToken.None);
                }
                catch (UnauthorizedAccessException)
                {
                    Error = "Não autorizado";
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }

            // refresh front end
            await InvokeAsync(StateHasChanged);
        }
    }
}