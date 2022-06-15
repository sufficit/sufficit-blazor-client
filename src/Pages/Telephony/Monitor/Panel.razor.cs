using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "telephony")]
    public partial class Panel : MonitorTelephonyBasePageComponent
    {
        protected override string Title => "Painel";

        protected override string Description => "Cartões de Recursos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private EventsPanelService Service { get; set; } = default!;

        protected Exception? ErrorConfig { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
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
                                var options = new AMIHubClientOptions() { Endpoint = new Uri(ep.Endpoint) };
                                var client = new AMIHubClient(options);
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
                    var options = await APIClient.Telephony.EventsPanel.GetServiceOptions();
                    if (options != null)
                    {
                        var weps = JsonSerializer.Serialize(options);
                        Console.WriteLine(weps);

                        Service.Panel.Cards.Clear();
                        Service.OnConfigure(options);
                    }
                    else { Console.WriteLine("null options received from api"); }
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }

                if (Service.CardAvatarHandler == null)
                {
                    try
                    {
                        Service.CardAvatarHandler = GetAvatarUrl;
                        await Service.StartAsync(System.Threading.CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        ErrorConfig = ex;
                    }
                }
            }
        }

        protected async Task<string> GetAvatarUrl(EventsPanelCard monitor)
        {
            return await Task.FromResult("https://www.sufficit.com.br/Relacionamento/Avatar.ashx");
        }
    }
}
