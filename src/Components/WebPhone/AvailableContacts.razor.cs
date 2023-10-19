using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Client;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class AvailableContacts : ComponentBase
    {
        [Inject]
        APIClientService APIClient { get; set; } = default!;

        [Inject]
        public JsSIPService JsSIPService { get; set; } = default!;

        /// <summary>
        ///     WebRTC current key
        /// </summary>
        [Parameter]
        [EditorRequired]
        public Guid Key { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public JsSIPSessions Sessions { get; set; } = default!;

        public IEnumerable<string>? Available { get; set; }

        protected Guid GetUserId(string key)
        {
            if (key.Length == 64)            
                if (Guid.TryParse(key.Substring(0, 32), out Guid userid))
                    return userid;
            
            return Guid.Empty;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // if (!firstRender) return;
            Available = await APIClient.Telephony.WebRTC.GetAvailable(CancellationToken.None);
            Available = Available.Where(x => !x.Contains(Key.ToString("N")));
        }

        protected async Task VoiceCall(string Destination)
        {
            var info = await JsSIPService.Call(Destination, false);
            Sessions.Monitor.Set(info.Id);
            await InvokeAsync(StateHasChanged);
        }
    }
}
