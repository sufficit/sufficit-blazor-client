using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Gateway.ReceitaNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    [Authorize(Roles = "telephony")]
    public partial class ReceitaNet : BasePageComponent, IDisposable
    {
        public const string RouteParameter = "pages/gateway/receitanet";

        protected override string Title => "ReceitaNet";

        protected override string Description => "Gateway de integração com o ReceitaNet";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        private async void ContextViewChanged(Guid? _)
        {
            await DataBind();
        }

        protected RNOptions Options { get; set; } = default!;

        protected ICollection<RNDestination> Destinations { get; set; } = default!;
                
        protected void Defaults()
        {
            // default empty settings
            Options = new RNOptions();
            Options.Title = "default";
            Destinations = new List<RNDestination>();
        }

        public ReceitaNet() { Defaults(); }

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

            var contextid = ContextView.ContextId.GetValueOrDefault();
            if (contextid != Guid.Empty)
            {
                await InvokeAsync(StateHasChanged);
                var options = await APIClient.Gateway.ReceitaNet.GetOptions(contextid);
                if (options != null) {
                    Options = options;
                    var destinations = (await APIClient.Gateway.ReceitaNet.GetDestinations(Options.Id, default))?.ToList();
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
                await APIClient.Gateway.ReceitaNet.Update(Options);
                await APIClient.Gateway.ReceitaNet.Update(Options.Id, Destinations);
            }
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
