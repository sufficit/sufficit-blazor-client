using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Telephony.JsSIP;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class Dialog : ComponentBase
    {
        [Parameter]
        public string? PhoneNumber { get; set; }

        [EditorRequired]
        [Parameter]
        public JsSIPSessionMonitor CallSession { get; set; } = default!;

        [Parameter]
        public EventCallback<string> SetIsCalling { get; set; }

        public JsSIPSessionEvent? Event
            => CallSession?.Event;

        protected override void OnAfterRender (bool firstRender)
        {
            if (!firstRender) 
                return;

            CallSession.OnChanged += OnCallSessionChanged;        
        }

        protected async ValueTask OnCallSessionChanged(JsSIPSessionMonitor sender)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
