using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk.Manager.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    [Authorize(Roles = Sufficit.Identity.ManagerRole.NormalizedName)]
    public partial class EventsData 
    {
        [Parameter]
        [EditorRequired]
        public IEnumerable<string> Items { get; set; } = default!;  
    }
}
