using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Blazor.Client.Components.Notification;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Notification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Notification
{
    public partial class ByEvent : BasePageComponent
    {

        public const string RouteParameter = "pages/notification/byevent";

        public const string? Icon = Icons.Material.Filled.Event;

        protected override string Title => "Por Evento";

        protected override string Description => "Assinantes por evento";


        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Inject]
        protected APIClientService APIClient { get; set; } = default!;


        [SupplyParameterFromQuery(Name = "eventid")]
        protected Guid? _eventid { get; set; }


        [SupplyParameterFromQuery(Name = "key")]
        protected string? _key { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (_eventid.GetValueOrDefault() != Guid.Empty)
            {
                if (Events == null)
                    Events = await APIClient.Notification.GetEvents(default);

                var info = Events.FirstOrDefault(s => s.Id == _eventid.GetValueOrDefault());
                if (info != null) await OnEventChanged(info);
            }

            if (!string.IsNullOrWhiteSpace(_key))
                Key = _key;

            await base.OnParametersSetAsync();
        }

        protected Sufficit.Notification.EventInfo? NEvent { get; set; }

        protected IEnumerable<EventInfo>? Events { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (Events == null)
            {
                Events = await APIClient.Notification.GetEvents(default);
                await InvokeAsync(StateHasChanged);
            }
        }

        public string? Key { get; set; }

        public bool ByKey => NEvent?.Method == SubscribeMethod.ByKey;

        InputType KeyInputType { get; set; }
        string? KeyAdornmentIcon { get; set; }
        string? KeyHelperText { get; set; } 

        protected async Task OnEventChanged(EventInfo selected)
        {
            if (selected != NEvent)
            {
                NEvent = selected;

                if (NEvent.Id.ToString() == "7ae23e1a-a1d6-4488-a50b-4eb54a424549")
                {
                    KeyInputType = InputType.Number;
                    KeyAdornmentIcon = @Icons.Material.Filled.MoneyOff;
                    KeyHelperText = "O evento será acionado quando o saldo para chamadas for inferior ao limite definido aqui.";
                    
                    if (string.IsNullOrWhiteSpace(Key))
                        Key = "50";
                } 
                else {
                    KeyInputType = default;
                    KeyAdornmentIcon = null;
                    KeyHelperText = null;
                    Key = null;
                }

                await InvokeAsync(StateHasChanged);
            }
        }

        protected ContactValidationResponse? Validation { get; set; }

        protected async Task OnValidate (ContactValidationResponse? response)
        {
            Validation = response;
            await InvokeAsync(StateHasChanged);
        }

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        NotificationContactValidate NotificationContactValidateRef = default!;

        protected async Task OnSubscribeClick(MouseEventArgs args)
        {
            if (Validation != null && NEvent != null)
            {
                string? key = null;
                if (NEvent.Method == SubscribeMethod.ByKey)
                    key = Key;

                var request = new SubscribeRequest()
                {
                    EventId = NEvent.Id,
                    ContextId = ContextView.ContextId,
                    Channel = Validation.Channel,
                    Destination = Validation.Destination,
                    Key = key
                };

                try
                {
                    await APIClient.Notification.Subscribe(request, default);
                    Snackbar.Add("Inscrição confirmada !", Severity.Success);

                    NotificationContactValidateRef.Refresh();
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception ex) 
                {
                    Snackbar.Add(ex.Message, Severity.Error);
                }

            }
        }
    }
}
