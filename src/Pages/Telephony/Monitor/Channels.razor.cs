﻿using Microsoft.AspNetCore.Authorization;
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
    public partial class Channels : MonitorTelephonyBasePageComponent, IDisposable
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

        private async void TextSearchValueChanged(string? value) 
        { 
            await InvokeAsync(StateHasChanged); 
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            Service.Channels.OnChanged += Channels_OnChanged;
            if (TextSearch != null)
                TextSearch.OnValueChanged += TextSearchValueChanged;
        }

        private async void Channels_OnChanged(IMonitor? sender, object? state)
        {            
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (TextSearch != null)            
                TextSearch.Toggle(true);            
        }

        void IDisposable.Dispose()
        {
            Service.Channels.OnChanged -= Channels_OnChanged;
            if(TextSearch != null)
                TextSearch.OnValueChanged -= TextSearchValueChanged;
        }
    }
}
