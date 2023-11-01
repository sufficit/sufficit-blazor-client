using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class ActiveSessions : ComponentBase, IDisposable
    {
        [Inject]
        protected BlazorContextRuntime BCRuntime { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public JsSIPSessions Sessions { get; set; } = default!;

        public IEnumerable<JsSIPSessionInfo> Active => Sessions.Where(s => !s.IsFinished());

        protected string AudioFile { get; set; }
            = "/assets/ringing-151670.mp3";

        private bool AudioPlayExists { get; set; }

        protected bool AudioPlay 
            =>!AudioPlayExists && Sessions.Where(s => s.Direction == JsSIPSessionDirection.incoming && s.Status.CanAnswer()).Any();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return; 
            Sessions.OnChanged += OnSessionsChanged;

            AudioPlayExists = await BCRuntime.HasElementWithId("sufficit-ringing");
            await InvokeAsync(StateHasChanged);
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

        protected async void OnSessionSelected(JsSIPSessionInfo info)
        {
            Sessions.Monitor.Set(info.Id);

            var destination = Pages.Telephony.WebPhone.RouteParameter;
            if (!Navigation.Uri.Contains(destination))
                Navigation.NavigateTo(destination, false);

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Sessions.OnChanged -= OnSessionsChanged;
        }
    }
}
