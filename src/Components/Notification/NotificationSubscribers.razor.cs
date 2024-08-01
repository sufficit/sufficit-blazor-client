using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Exchange;
using Sufficit.Identity;
using Sufficit.Notification;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Notification
{
    public partial class NotificationSubscribers : ComponentBase
    {
        [Inject]
        protected APIClientService APIClient { get; set; } = default!;

        [CascadingParameter]
        public UserPrincipal User { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public Guid? EventId { get; set; }

        Guid? LastEventId { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            if (EventId != LastEventId)
            {
                LastEventId = EventId;
                if (EventId.GetValueOrDefault() != Guid.Empty)
                {


                }
            }

            await base.OnParametersSetAsync();
        }
    }
}
