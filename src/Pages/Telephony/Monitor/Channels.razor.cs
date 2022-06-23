using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = "manager")]
    public partial class Channels : MonitorTelephonyBasePageComponent
    {
        protected override string Title => "Canais";

        protected override string Description => "Filtro de canais ativos";

        [Inject]
        EventsPanelService Service { get; set; } = default!;

        protected int Count => Service.Channels.Count;

        protected string? FilterText => TextSearch?.Value;

        protected IEnumerable<ChannelInfoMonitor> GetItems()
        {
            var items = Service.Channels;
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                string filter = FilterText.ToLowerInvariant().Trim();
                foreach (var item in items.Where(s => s.Key.Contains(filter)))
                    yield return item;
            }
            else
            {
                foreach (var item in items)
                    yield return item;
            }
        }

        protected Exception? ErrorConfig { get; set; }

        public int MaxItems { get; set; } = 20;

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        private void TextSearchValueChanged(string? value) 
        { 
            StateHasChanged(); 
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();            
            try
            {
                await Service.StartAsync(System.Threading.CancellationToken.None);
            }
            catch (Exception ex)
            {
                ErrorConfig = ex;
            }            

            Service.Channels.OnChanged += Channels_OnChanged;
        }

        private async void Channels_OnChanged(IMonitor? sender, object? state)
        {            
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (TextSearch != null)
            {
                TextSearch.Toggle(true);
                TextSearch.OnValueChanged += TextSearchValueChanged;
            }
        }
    }
}
