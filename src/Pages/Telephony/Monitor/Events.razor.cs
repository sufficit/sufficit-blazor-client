using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "manager")]
    public partial class Events : MonitorTelephonyBasePageComponent
    {
        protected override string Title => "Eventos";

        protected override string Description => "Filtro de eventos";

        [Inject]
        private EventsPanelService Service { get; set; } = default!;

        protected Exception? ErrorConfig { get; set; }

        public Queue<string> Items { get; }

        public int MaxItems { get; set; } = 20;

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        public Events()
        {
            Items = new Queue<string>(MaxItems);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                
            }
        }

        private async void TextSearchValueChanged(string? value) 
        { 
            Items.Clear(); 
            await InvokeAsync(StateHasChanged); 
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Service != null)
            {
                try
                {
                    await Service.StartAsync(System.Threading.CancellationToken.None);
                }
                catch (Exception ex)
                {
                    ErrorConfig = ex;
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(Service != null)
            {
                Service.OnEvent += Service_OnEvent;
            }

            if (TextSearch != null)
            {
                TextSearch.Toggle(true);
                TextSearch.OnValueChanged += TextSearchValueChanged;
            }
        }

        private async void Service_OnEvent(object sender, Asterisk.Manager.Events.IManagerEventFromAsterisk e)
        {
            var json = e.GetType() + " :: " + JsonSerializer.Serialize(e, e.GetType());
            if (!string.IsNullOrEmpty(TextSearch?.Value))
            {
                if (!json.Contains(TextSearch?.Value!))
                {
                    return;
                }
            }

            if (Items.Count == MaxItems)
                Items.Dequeue();
            
            Items.Enqueue(json);
            await InvokeAsync(StateHasChanged);
        }
    }
}
