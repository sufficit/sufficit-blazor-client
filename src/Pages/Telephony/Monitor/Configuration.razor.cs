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
    public partial class Configuration : MonitorTelephonyBasePageComponent
    {
        protected override string Title => "Configurações";

        protected override string Description => "Opções do painel de eventos";

        [Inject]
        private APIClientService? APIClient { get; set; }

        protected EventsPanelUserOptions? Options { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            Options = await APIClient.Telephony.EventsPanel.GetUserOptions();
        }

        protected async Task OnCheckBoxChanged(ChangeEventArgs e)
        {
            if (Options != null)
            {
                if (bool.TryParse(e.Value?.ToString(), out bool newValue)) 
                {
                    Options.ShowTrunks = newValue;
                    await APIClient.Telephony.EventsPanel.PostUserOptions(Options); 
                }
            }
        }
    }
}
