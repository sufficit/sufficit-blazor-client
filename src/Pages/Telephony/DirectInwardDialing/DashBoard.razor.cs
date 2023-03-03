using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class DashBoard : TelephonyBasePageComponent, IDisposable, IPage
    {
        public const string RouteParameter = "/pages/telephony/did/dashboard";

        protected override string Title => "Direct Inward Dialing";

        protected override string Description => "Rotas de entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }

        private async void ContextViewChanged(Guid obj)
        {
            await GetItems();
        }

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contact.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected IEnumerable<Sufficit.Telephony.DirectInwardDialing>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            _ = await ContextView.Default();
            
            // getting items for the first time
            await GetItems();

            // if change, get items again
            ContextView.OnChanged += ContextViewChanged;
        }

        protected async Task GetItems()
        {
            IsLoading = true;     
            if (ContextView.ContextId != Guid.Empty)
            {
                await InvokeAsync(StateHasChanged);
                Items = await APIClient.Telephony.DID.ByContext(ContextView.ContextId);                
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
