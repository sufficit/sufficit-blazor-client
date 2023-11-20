using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Telephony.DIDs;
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
        public const string RouteParameter = "pages/telephony/did/dashboard";

        public const string? Icon = Icons.Material.Filled.TripOrigin;

        protected override string Title => "Direct Inward Dialing";

        protected override string Description => "Rotas de entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public string? Filter { get; set; }

        public DIDSearchParameters? Parameters { get; set; }

        protected void OnTextChanged(string? value)
        {
            Filter = value;
            
        }

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }

        private async void ContextViewChanged(Guid obj)
        {
            Parameters ??= new DIDSearchParameters();
            Parameters.ContextId = ContextView.ContextId;

            await InvokeAsync(StateHasChanged);
        }

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contacts.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected IEnumerable<Sufficit.Telephony.DirectInwardDialing>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            // if change, get items again
            ContextView.OnChanged += ContextViewChanged;

            _ = await ContextView.Default();

            Parameters ??= new DIDSearchParameters();
            Parameters.ContextId = ContextView.ContextId;
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
