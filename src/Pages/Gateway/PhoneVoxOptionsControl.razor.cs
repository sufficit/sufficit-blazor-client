using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Gateway.PhoneVox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    [Authorize(Roles = "telephony")]
    public partial class PhoneVoxOptionsControl : ComponentBase, IDisposable
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? GatewayId { get; set; }

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        private async void ContextViewChanged(Guid? _)
        {
            await DataBind();
        }

        protected PhoneVoxOptions Options { get; set; } = default!;

        protected ICollection<PhoneVoxDestination> Destinations { get; set; } = default!;

        protected void Defaults()
        {
            // default empty settings
            Options = new PhoneVoxGateway();
            Options.Server.Title = "default";

            Destinations = new List<PhoneVoxDestination>();
        }

        public PhoneVoxOptionsControl() => Defaults();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            _ = await ContextView.Default();

            // getting items for the first time
            await DataBind();

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

            // clearing form
            Defaults();

            await InvokeAsync(StateHasChanged);

            if (GatewayId.HasValue) 
            { 
                var options = await APIClient.Gateway.PhoneVox.GetById(GatewayId.Value);
                if (options != null)
                {
                    Options = options;
                    var destinations = (await APIClient.Gateway.PhoneVox.GetDestinations(GatewayId.Value, default))?.ToList();
                    if (destinations != null)
                        Destinations = destinations;
                }
            } 

            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Saving changes
        /// </summary>
        protected async Task Save()
        {
            if (ContextView.ContextId.GetValueOrDefault() != Guid.Empty)
            {
                await APIClient.Gateway.PhoneVox.Update(Options);
                await APIClient.Gateway.PhoneVox.Update(Options.Id, Destinations);
            }
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
