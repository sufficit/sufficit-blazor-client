using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Identity;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
    public partial class Events : MonitorTelephonyBasePageComponent, IDisposable, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/monitor/events";

        public const string Title = "Eventos";
        protected override string Description => "Filtro de eventos";

        #endregion

        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        [Parameter]
        public string? Filter { get; set; }

        protected Exception? ErrorConfig { get; set; }

        public IEnumerable<string> Items { get { lock (_lock) return _items.ToList(); } }

        private readonly Queue<string> _items;
        private readonly object _lock;

        public int MaxItems { get; set; } = 20;


        public Events()
        {
            _items = new Queue<string>(MaxItems);
            _lock = new object();
        }
        
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            EPService.OnEvent += OnEPServiceEvent;
        }

        private async void OnEPServiceEvent(object sender, Asterisk.Manager.Events.IManagerEventFromAsterisk e)
        {            
            var json = e.GetType() + " :: " + JsonSerializer.Serialize(e, e.GetType());
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                if (!json.Contains(Filter, StringComparison.InvariantCultureIgnoreCase))
                    return;
            }

            lock (_lock)
            {             
                if (_items.Count == MaxItems)
                    _items.Dequeue();

                _items.Enqueue(json);
            }

            await InvokeAsync(StateHasChanged);            
        }

        protected async void OnTextChanged(string? value)
        {            
            if (value != null)
            {
                lock (_lock)
                {
                    foreach (var json in Filtering().ToList())
                    {
                        if (json.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                            _items.Enqueue(json);
                    }
                }
            }

            await InvokeAsync(StateHasChanged);            
        }

        private IEnumerable<string> Filtering()
        {
            lock (_lock)            
                while (_items.TryDequeue(out string? json))
                    yield return json;            
        }

        void IDisposable.Dispose()
        {
            EPService.OnEvent -= OnEPServiceEvent;
        }
    }
}
