using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Blazor.UI.Material;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.CheckUp
{
    [Authorize(Roles = "manager")]
    public partial class OutBoundRoutes : TelephonyBasePageComponent
    {
        [Inject]
        IWebSocketService WSClient { get; set; } = default!;

        [Inject]
        IContextView View { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "ContextId")]
        public Guid? ContextId { get; set; }

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        protected bool ClientSearchVisible { get; set; }

        protected override string Title => "Rotas de saída";

        protected override string Description => "Confere se as rotas de saída estão devidamente configuradas";

        protected override Task OnInitializedAsync()
        {
            WSClient.OnChanged += WSClient_OnChanged;
            WSClient.Test();

            return base.OnInitializedAsync();
        }

        private async void WSClient_OnChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(ContextId.HasValue && ContextId.Value != Guid.Empty)
            {
                TextSearch?.Toggle(false);
                View.Update(ContextId.Value);
            } else
            {
                if (View.ContextId == Guid.Empty)
                    View.Update(Guid.Parse("d21cfb04-9d37-473b-837c-67591a26feed"));
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (TextSearch != null)
                {
                    TextSearchValueChanged(TextSearch.Value);
                    TextSearch.OnValueChanged += TextSearchValueChanged;
                }

                await DoCheckUp();
            }
        }

        private async void TextSearchValueChanged(string? obj)
        {
            if (ClientSearchVisible)
            {
                if(string.IsNullOrWhiteSpace(obj))
                {
                    ClientSearchVisible = false;
                    await InvokeAsync(StateHasChanged);
                }
            } else
            {
                if (!string.IsNullOrWhiteSpace(obj))
                {
                    ClientSearchVisible = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected IList<CheckUpStepInfo> Infos { get; } = new List<CheckUpStepInfo>();

        public async Task DoCheckUp()
        {
            Infos.Clear();
            var cts = new CancellationTokenSource();
            await foreach(var step in WSClient.CheckUpOutBoundRoutes(View.ContextId, cts.Token))
            {
                Infos.Add(step);
                await InvokeAsync(StateHasChanged);
            }
        }

        public async void OnClientSelect(Guid id)
        {
            await View.Update(id);

            // Cleating filters
            TextSearch?.Update(null);
            await DoCheckUp();
        }
    }
}
