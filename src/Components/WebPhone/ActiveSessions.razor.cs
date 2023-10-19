using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class ActiveSessions : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public JsSIPSessions Sessions { get; set; } = default!;

        public IEnumerable<JsSIPSessionInfo> Active => Sessions.Where(s => !s.IsFinished());

        protected string AudioFile { get; set; }
            = "/assets/ringing-151670.mp3";

        protected bool AudioPlay 
            => Sessions.Where(s => s.Direction == JsSIPSessionDirection.incoming && s.Status.CanAnswer()).Any();

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            Sessions.OnChanged += OnSessionsChanged;
        }

        protected Guid GetUserId(string? key)
        {
            if (key != null && key.Length == 64)
                if (Guid.TryParse(key.Substring(0, 32), out Guid userid))
                    return userid;

            return Guid.Empty;
        }

        private async void OnSessionsChanged(object? sender, System.EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Sessions.OnChanged -= OnSessionsChanged;
        }
    }
}
