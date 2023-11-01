using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Telephony;
using System;
using System.Collections;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class WebPhoneIconButtonMenu : ComponentBase, IDisposable
    {
        [Inject]
        protected WebRTCControl Control { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            Control.OnChanged += OnControlChanged;
        }

        private async void OnControlChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Control.OnChanged -= OnControlChanged;
        }

        protected MudBlazor.Color Color 
        { 
            get
            {
                if (!Control.IsConnected) return Color.Dark;
                if (!Control.IsRegistered) return Color.Info;
                return Color.Primary;
            } 
        }
    }
}
