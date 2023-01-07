using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class DashBoard : TelephonyBasePageComponent, IDisposable
    {
        protected override string Title => "Direct Inward Dialing";

        protected override string Description => "Rotas de entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;
                
        private async void ContextViewChanged(Guid obj)
        {
            await GetItems();
        }

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contact.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected IEnumerable<Sufficit.Telephony.DirectInwardDialingV1>? Items { get; set; }

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
            if(ContextView.ContextId != Guid.Empty)
            {
                Items = await APIClient.Telephony.DID.ByContext(ContextView.ContextId);                
            }

            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
