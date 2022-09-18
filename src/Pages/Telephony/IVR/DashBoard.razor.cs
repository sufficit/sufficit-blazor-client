using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Client;
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
        protected override string Title => "IVR";

        protected override string Description => "Opções do painel de eventos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await GetItems();
        }
        
        private async void ContextViewChanged(Guid obj)
        {
            await GetItems();
        }

        protected IEnumerable<Sufficit.Telephony.IVR>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;            
            
            ContextView.OnChanged += ContextViewChanged;
            _ = await ContextView.Default();
        }

        protected async Task GetItems()
        {
            if(ContextView.ContextId != Guid.Empty)
            {
                Items = await APIClient.Telephony.IVR.ByContext(ContextView.ContextId);                
            }

            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
