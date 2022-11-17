using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "manager")]
    public partial class Events : MonitorTelephonyBasePageComponent, IDisposable
    {
        protected override string Title => "Eventos";

        protected override string Description => "Filtro de eventos";

        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        protected Exception? ErrorConfig { get; set; }

        public Queue<string> Items { get { lock (_lock) return _items; } }

        private readonly Queue<string> _items;
        private readonly object _lock;

        public int MaxItems { get; set; } = 20;


        public Events()
        {
            _items = new Queue<string>(MaxItems);
            _lock = new object();
        }
        /*
        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            EPService.OnEvent += Service_OnEvent;

            if (TextSearch != null)
                TextSearch.OnValueChanged += TextSearchValueChanged;
        }
        */
        private async void TextSearchValueChanged(string? value) 
        { 
            lock(_lock)
                _items.Clear(); 

            await InvokeAsync(StateHasChanged); 
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            /*
            if (TextSearch != null)            
                TextSearch.Toggle(true);    
            */
        }

        private async void Service_OnEvent(object sender, Asterisk.Manager.Events.IManagerEventFromAsterisk e)
        {
            /*
            var json = e.GetType() + " :: " + JsonSerializer.Serialize(e, e.GetType());
            if (!string.IsNullOrEmpty(TextSearch?.Value))
            {
                if (!json.Contains(TextSearch?.Value!))
                {
                    return;
                }
            }

            lock (_lock)
            {
                if (_items.Count == MaxItems)
                    _items.Dequeue();

                _items.Enqueue(json);
            }
            await InvokeAsync(StateHasChanged);
            */
        }

        void IDisposable.Dispose()
        {
            EPService.OnEvent -= Service_OnEvent;
            /*
            if (TextSearch != null)
                TextSearch.OnValueChanged -= TextSearchValueChanged;
            */
        }
    }
}
