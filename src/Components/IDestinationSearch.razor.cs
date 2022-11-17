using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class IDestinationSearch : ComponentBase
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Parameter]
        public Guid? ContextId { get; set; }

        [Parameter]
        public IDestination? Value { get; set; }

        [Parameter]
        public string? Asterisk { 
            set 
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    UpdateAsterisk(value);
                }
            } 
        }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public EventCallback<IDestination?> ValueChanged { get; set; }

        protected string? GetAsterisk() => Value?.Asterisk;

        protected string? GetHelperText() => Value != null ? $"({Value.TypeName}) {Value.Description}" : null;

        protected string? GetAdornmentIcon()
        {
            if (Value == null) return Icons.Material.Filled.Search;
            switch (Value.TypeName.Trim().ToLowerInvariant())
            {
                case "mailbox":
                case "freepbxmailbox": return Icons.Material.Filled.Voicemail;
                case "enddestination": return Icons.Material.Filled.CallEnd;
                case "directextensiondialing":
                case "diddirect": return Icons.Material.Filled.Fax;
                default: return Icons.Material.Filled.QuestionMark;
            }
        }

        protected async Task<IEnumerable<IDestination>> Search(string filter)
        {
            var parameters = new DestinationSearchParameters()
            {
                ContextId = this.ContextId ?? Guid.Empty,
                Filter = filter,
                Limit = 5
            };
            
            return await APIClient.Telephony.Destinations(parameters);
        }

        protected async Task DestinationValueChanged(IDestination destination)
        {
            if (Value != destination)
            {
                Value = destination;

                if(ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
            }
        }

        protected async void UpdateAsterisk(string asterisk)
        {
            if (!string.IsNullOrWhiteSpace(asterisk) && asterisk != Value?.Asterisk)
            {
                var parameters = new DestinationSearchParameters()
                {
                    ContextId = this.ContextId ?? Guid.Empty,
                    Filter = asterisk
                };

                var value = await APIClient.Telephony.Destination(parameters);
                if(value == null) 
                {
                    value = new Destination() {
                        Asterisk = asterisk,
                        Title = asterisk,
                        TypeName = "Unknown",
                    }; 
                }
                Value = value;

                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
