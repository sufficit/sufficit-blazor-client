﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.CheckUp;
using Sufficit.Client;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.CheckUp
{
    [Authorize(Roles = ManagerRole.NormalizedName)]
    public partial class OutBoundRoutes : TelephonyBasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/checkup/outboundroutes";

        public const string Title = "Rotas de saída";
        protected override string Description => "Confere se as rotas de saída estão devidamente configuradas";

        #endregion
    
        [Inject]
        IWebSocketService WSClient { get; set; } = default!;

        [Inject]
        IContextView ContextView { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "ContextId")]
        public Guid? ContextId { get; set; }

        /*
        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }
        */

        protected bool ClientSearchVisible { get; set; }

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
            /*
            if(ContextId.HasValue && ContextId.Value != Guid.Empty)
            {
                TextSearch?.Toggle(false);
                ContextView.Update(ContextId.Value);
            } else
            {
                if (ContextView.ContextId.GetValueOrDefault() == Guid.Empty)
                    ContextView.Update(Guid.Parse("d21cfb04-9d37-473b-837c-67591a26feed"));
            }
            */
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            /*
            if (firstRender)
            {
                if (TextSearch != null)
                {
                    TextSearchValueChanged(TextSearch.Value);
                    TextSearch.OnValueChanged += TextSearchValueChanged;
                }

                await DoCheckUp();
            }
            */
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
            await foreach(var step in WSClient.CheckUpOutBoundRoutes(ContextView.ContextId.GetValueOrDefault(), cts.Token))
            {
                Infos.Add(step);
                await InvokeAsync(StateHasChanged);
            }
        }

        public void OnClientSelect(Guid id)
        {
            ContextView.Update(id);
            /*
            // Cleating filters
            TextSearch?.Update(null);
            await DoCheckUp();
            */
        }
    }
}
