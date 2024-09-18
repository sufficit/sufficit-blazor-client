using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Identity;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = ManagerRole.NormalizedName)]
    public partial class Channels : MonitorTelephonyBasePageComponent, IDisposable, IPage
    {
        #region INTERFACE IPAGE
        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/monitor/channels";

        public const string Title = "Canais";
        protected override string Description => "Filtro de canais ativos";

        #endregion

        [Inject]
        EventsPanelService Service { get; set; } = default!;

        protected int Count => Service.Channels.Count;

        //protected string? FilterText => TextSearch?.Value;

        protected IEnumerable<ChannelInfoMonitor> GetItems()
        {            
            var items = Service.Channels;
            /*
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                string filter = FilterText.ToLowerInvariant().Trim();
                foreach (var item in items.Where(s => s.Key.Contains(filter)))
                    yield return item;
            }
            else
            */
            {
                foreach (var item in items)
                    yield return item;
            }
            
        }

        protected Exception? ErrorConfig { get; set; }

        public int MaxItems { get; set; } = 20;
               
        private async void TextSearchValueChanged(string? value) 
        { 
            await InvokeAsync(StateHasChanged); 
        }

        /*
        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }
        */

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            Service.Channels.OnChanged += OnChannelsCollectionChanged;
        }
        

        private async void OnChannelsCollectionChanged(IMonitor? sender, object? state)
        {            
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

        void IDisposable.Dispose()
        {
            Service.Channels.OnChanged -= OnChannelsCollectionChanged;

            /*
            if(TextSearch != null)
                TextSearch.OnValueChanged -= TextSearchValueChanged;
            */
        }
    }
}
