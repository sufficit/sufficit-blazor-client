using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Gateway.PhoneVox;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    [Authorize(Roles = "telephony")]
    public partial class PhoneVox : BasePageComponent, IDisposable
    {
        public const string RouteParameter = "pages/gateway/phonevox";

        protected override string? Area => "Gateway";

        protected override string Title => "PhoneVox";

        protected override string Description => "Gateway de integração com a PhoneVox";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? GatewayId { get; set; }

        public IEnumerable<PhoneVoxOptions> Options { get; set; }
            = new List<PhoneVoxOptions>();

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        private async void ContextViewChanged(Guid? _)
        {
            await DataBind();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            _ = await ContextView.Default();

            // if change, get items again
            ContextView.OnChanged += ContextViewChanged;
        }

        /// <summary>
        /// Retrieving data
        /// </summary>
        /// <returns></returns>
        protected async Task DataBind()
        {
            IsLoading = true;

            var contextid = ContextView.ContextId.GetValueOrDefault();
            if (contextid != Guid.Empty)
            {
                await InvokeAsync(StateHasChanged);

                if (GatewayId.HasValue) 
                { 
                    Options = await APIClient.Gateway.PhoneVox.GetByContextId(GatewayId.Value);
                } 
            }

            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
