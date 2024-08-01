using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Blazor.Client.Shared.Tables;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

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

        [Inject]
        IDialogService Dialog { get; set; } = default!;

        [CascadingParameter]
        public UserPrincipal User { get; set; } = default!;

        public DIDSearchParameters? Parameters { get; set; }

        protected DIDTable TableReference { get; set; } = default!;

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }

        private async void ContextViewChanged(Guid? value)
        {
            Parameters ??= new DIDSearchParameters();
            Parameters.ContextId = value;

            await TableReference.DataBind();
        }

        protected static string ToE164Semantic(string extension)
            => Sufficit.Telephony.Utils.FormatToE164Semantic(extension);

        protected async Task<string> GetTitle(Guid id)
            => (await APIClient.Contacts.GetContact(id, default))?.Title ?? "*Desconhecido";

        protected IEnumerable<Sufficit.Telephony.DirectInwardDialing>? Items { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            _ = await ContextView.Default<Sufficit.Telephony.TelephonyAdminDirective>();            
            Parameters ??= new DIDSearchParameters();
            Parameters.ContextId = ContextView.ContextId;            
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            // if change, get items again
            ContextView.OnChanged += ContextViewChanged;
            Navigation.LocationChanged += OnLocationChanged;
        }

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            Console.WriteLine($"Location changed: {e.Location}");
            await InvokeAsync(StateHasChanged);
        }

        protected async Task OnContextSelected(Guid contextId)
        {
            if (ContextView.ContextId != contextId)
                await ContextView.Update(contextId);
        }

        protected void OnContextSearchRequested()
        {
            Dialog.Show<ContextFilterDialog>();
        }

        void IDisposable.Dispose()
        {
            ContextView.OnChanged -= ContextViewChanged;
            Navigation.LocationChanged -= OnLocationChanged;
        }
    }
}
