using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public partial class Panel : MonitorTelephonyBasePageComponent, IDisposable
    {
        protected override string Title => "Painel";

        protected override string Description => "Cartões de Recursos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private EventsPanelService Service { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Inject]
        private IServiceProvider Provider { get; set; } = default!;

        protected Sufficit.Telephony.EventsPanel.Panel? PanelCurrent { get; set; }


        protected Exception? ErrorConfig { get; set; }

        protected bool IsRendered { get; set; }

        protected override void OnParametersSet()
        {            
            ContextView.OnChanged += ContextView_OnChanged;            
        }

        private async void ContextView_OnChanged(Guid contextId)
        {
            if (IsRendered)
            {
                await LoadPanel(contextId, default);                
            }
        }

        public void Dispose() 
        {            
            ContextView.OnChanged -= ContextView_OnChanged;            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) 
                return;

            if (!Service.IsConfigured)
            {
                try
                {
                    var endpoints = await APIClient.Telephony.EventsPanel.GetEndpoints();
                    if (endpoints != null)
                    {
                        var ep = endpoints.FirstOrDefault();
                        if (ep != null)
                        {
                            //Console.WriteLine($"configuring with endpoint: {ep.Endpoint}");
                            //var amiOptions = Options.Create(new AMIHubClientOptions() { Endpoint = new Uri(ep.Endpoint) });
                            //var amiLogger = Provider.GetRequiredService<ILogger<AMIHubClient>>();
                            var scope = Provider.CreateScope();
                            var client = scope.ServiceProvider.GetRequiredService<AMIHubClient>();
                            Service.Configure(client);
                            ErrorConfig = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }
            }

            // Getting cards
            try
            {          
                await LoadPanel(ContextView.ContextId, default);
            }
            catch (Exception ex)
            {
                ErrorConfig = ex;
            }

            if (Service.CardAvatarHandler == null)
                Service.CardAvatarHandler = GetAvatarUrl;

            IsRendered = true;
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

            if (Service.Options != null) {
                Service.Options.AutoFill = false;
                Service.Options.ShowTrunks = false;
            }
            PanelCurrent = new Sufficit.Telephony.EventsPanel.Panel(cards, Service);       
            await InvokeAsync(StateHasChanged);
        }

        protected async Task<string> GetAvatarUrl(EventsPanelCard monitor)
        {
            return await Task.FromResult("https://www.sufficit.com.br/Relacionamento/Avatar.ashx");
        }
    }
}
