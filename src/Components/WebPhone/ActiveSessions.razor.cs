using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        [Parameter]
        [EditorRequired]
        public JsSIPSessions Sessions { get; set; } = default!;

        public IEnumerable<JsSIPSessionInfo> Active => Sessions.Where(s => s.Cause is null);

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            Sessions.OnChanged += OnSessionsChanged;
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
