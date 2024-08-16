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

namespace Sufficit.Blazor.Client.Pages.Telephony.IVR
{
    [Authorize(Roles = "telephony")]
    public partial class DashBoard : TelephonyBasePageComponent, IDisposable
    {
        public const string RouteParameter = "/pages/telephony/ivr/dashboard";

        public const string? Icon = Icons.Material.Filled.List;

        protected override string Title => "IVR";

        protected override string Description => "Opções do painel de eventos";

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
            await GetItems();
        }

        protected static string GetLinkForObject(Guid ivrid)
            => $"{Pages.Telephony.IVR.Object.RouteParameter}?{IVRSearchParameters.IVRID}={ivrid}";

        protected IEnumerable<Sufficit.Telephony.IVR>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            await ContextView.Default();

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
                Items = await APIClient.Telephony.IVR.ByContext(ContextView.ContextId.GetValueOrDefault());                
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
