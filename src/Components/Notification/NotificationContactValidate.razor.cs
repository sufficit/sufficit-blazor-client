using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Exchange;
using Sufficit.Notification;
using Sufficit.Sales;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Notification
{
    public partial class NotificationContactValidate : ComponentBase
    {
        [Inject]
        protected APIClientService APIClient { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "destination")]
        protected string? _destination { get; set; }

        [SupplyParameterFromQuery(Name = "channel")]
        protected string? _channel { get; set; }

        /// <summary>
        ///     Contact Validation Response, can be null after a refresh
        /// </summary>
        [Parameter]
        public EventCallback<ContactValidationResponse?> OnValidate { get; set; }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(_destination))   
                Destination = _last_destination = _destination;

            if (!string.IsNullOrWhiteSpace(_channel))            
                Channel = Enum.Parse<TChannel>(_channel, true);            
            
            base.OnParametersSet();
        }

        public string? Destination { get; set; }

        public TChannel Channel { get; set; }


        private string? _last_destination;
        protected void OnContactTextChanged (string value)
        {
            if (value != _last_destination)
            {
                _last_destination = value;

                Sufficit.Exchange.Utils.TryGetChannel(value, out TChannel channel);
                Channel = channel;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (!string.IsNullOrWhiteSpace(Destination))
                await OnValidateClick(new MouseEventArgs());
        }

        protected MudBlazor.Severity ContactValidationSeverity => ContactValidation?.Success ?? false ? MudBlazor.Severity.Success : MudBlazor.Severity.Error;

        ContactValidationResponse? ContactValidation { get; set; }
        protected async Task OnValidateClick (MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(Destination))
                return;

            var request = new ContactValidationRequest()
            {
                Channel = Channel,
                Destination = Destination,
            };

            var validation = ContactValidation = await APIClient.Notification.Contact.Validate(request, CancellationToken.None);

            if (OnValidate.HasDelegate)
                await OnValidate.InvokeAsync(validation);

            await InvokeAsync(StateHasChanged);
        }

        public async void Refresh()
        {
            Destination = null;
            Channel = default;
            ContactValidation = null;

            if (OnValidate.HasDelegate)
                await OnValidate.InvokeAsync(null);

            await InvokeAsync(StateHasChanged);
        }
    }
}
