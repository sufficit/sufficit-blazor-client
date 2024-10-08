﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "telephony")]
    public partial class Panel : MonitorTelephonyBasePageComponent, IDisposable, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;

        public const string RouteParameter = "/pages/telephony/monitor/panel";

        public const string Title = "Painel";
        protected override string Description => "Cartões de Recursos";

        #endregion

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private EventsPanelService Service { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;


        protected Sufficit.Telephony.EventsPanel.Panel? PanelCurrent { get; set; }


        protected Exception? ErrorConfig { get; set; }

        protected bool IsRendered { get; set; }

        protected override void OnParametersSet()
        {            
            base.OnParametersSet();

            ContextView.OnChanged += ContextView_OnChanged;            
        }

        private async void ContextView_OnChanged(Guid? contextId)
        {
            if (IsRendered)
            {
                await LoadPanel(contextId.GetValueOrDefault(), default);                
            }
        }

        public override void Dispose (bool disposing)
        {
            IsRendered = false;
            ContextView.OnChanged -= ContextView_OnChanged;

            // following to base dispose
            base.Dispose(disposing);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            IsRendered = true;
            if (!firstRender) return;

            if (!Service.IsConfigured)
            {
                try
                {
                    var endpoints = await APIClient.Telephony.EventsPanel.GetEndpoints();
                    if (endpoints != null)                    
                        Service.Configure(endpoints);                    
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }
            }

            // Getting cards
            try
            {          
                await LoadPanel(ContextView.ContextId.GetValueOrDefault(), default);
            }
            catch (Exception ex)
            {
                ErrorConfig = ex;
            }

            await InvokeAsync(StateHasChanged);
        }

        protected async Task LoadPanel(Guid contextId, CancellationToken cancellationToken)
        {
            IEnumerable<EventsPanelCardInfo> info;
            if (contextId != Guid.Empty)            
                info = await APIClient.Telephony.EventsPanel.GetCardsByContext(contextId, cancellationToken);            
            else
                info = await APIClient.Telephony.EventsPanel.GetCardsByUser(cancellationToken);            

            var cards = new EventsPanelCardCollection();
            foreach (var card in info)
            {
                var cardMonitor = EventsPanelCardExtensions.CardCreate(card, Service);
                cards.Add(cardMonitor);
            }

            PanelCurrent = new Sufficit.Telephony.EventsPanel.Panel(cards, Service);
            PanelCurrent.Cards.CardAvatarHandler = GetAvatarUrl;

            await InvokeAsync(StateHasChanged);
        }

        protected async Task<string> GetAvatarUrl(EventsPanelCardInfo monitor)
        {
            return await Task.FromResult("https://endpoints.sufficit.com.br/contact/avatar?contextid=00000000-0000-0000-0000-000000000000");
        }
    }
}
