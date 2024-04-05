using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using System;
using System.Threading.Tasks;
using MudBlazor;
using System.Collections;
using Sufficit.Notification;
using System.Collections.Generic;
using System.Linq;
using Sufficit.Blazor.Components;

namespace Sufficit.Blazor.Client.Components.MenuItems
{
    public partial class NotificationsIconMenuItem : ComponentBaseWithExceptionControl, IDisposable
    {
        [Inject]
        protected APIClientService APIClient { get; set; } = default!;

        protected const int MaxItems = 3;

        protected int Count => Notifications.Count();

        protected string Elapsed(DateTime timestamp)
            => DateTime.UtcNow.Subtract(timestamp).ToNaturalLanguage();        

        protected string Icon {
            get
            {

                if (Count > 0)
                {
                    if (Count > MaxItems) return Icons.Material.Filled.NotificationsActive;
                    return MudBlazor.Icons.Material.Filled.Notifications;
                }

                if (!APIClient.Available)
                    return Icons.Material.Outlined.NotificationsOff;

                return MudBlazor.Icons.Material.Filled.NotificationsNone;
            }
        }

        protected HashSet<BoardNotification> Notifications { get; set; } = new HashSet<BoardNotification>();

        protected MudBlazor.Color Color
        {
            get
            {
                if (!APIClient.Available)
                    return Color.Error;

                if (Count > 0)
                {
                    if (Count > MaxItems) return Color.Warning;
                    return Color.Info;
                }
                return Color.Default;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await APIClient.GetStatus();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!APIClient.Available) return;

            IEnumerable<BoardNotification> notifications;
            try
            {
                notifications = await APIClient.Notification.GetNotifications(default);
            }
            catch (Exception ex) 
            {
                this.Throw(ex);
                return;
            }

            var hs = notifications.ToHashSet();
            if (!hs.SetEquals(Notifications))
            {
                Notifications = hs;
            }
        }

        private async void OnControlChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose(bool disposing) 
        { 
            APIClient.OnChanged -= OnControlChanged;
        }
    }
}
