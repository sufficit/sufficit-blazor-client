using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public string? Asterisk { get; set; }

        /// <summary>
        ///     (optional) Label for textbox
        /// </summary>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public EventCallback<IDestination?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<string?> AsteriskChanged { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object?> Attributes { get; set; } = [];

        protected override void OnParametersSet()
        {
            if (Asterisk != null)            
                UpdateInformation(Asterisk);

            Attributes["data-asterisk"] = GetAsterisk();
        }

        protected string? GetLabel() => Label ?? "Destino";

        protected string? GetAsterisk() => Value?.Asterisk;

        protected string? GetHelperText() => Value != null ? $"({Value.TypeName}) {Value.Description}" : null;

        protected string? GetAdornmentIcon()
        {
            if (Value == null) return Icons.Material.Filled.Search;
            return Value.GetIcon();
        }

        protected async Task<IEnumerable<IDestination>> Search(string filter, CancellationToken cancellationToken)
        {
            var parameters = new DestinationSearchParameters()
            {
                ContextId = this.ContextId ?? Guid.Empty,
                Filter = filter,
                Limit = 5
            };
            
            return await APIClient.Telephony.Destination.Search(parameters, cancellationToken);
        }

        protected async Task OnValueChanged(IDestination destination)
        {
            if (Value != destination)
            {
                Value = destination;

                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);

                await OnAsteriskChanged(Value?.Asterisk);
            }
        }

        protected async Task OnAsteriskChanged(string? asterisk)
        {
            if (Asterisk != asterisk)
            {
                Asterisk = asterisk;

                if (AsteriskChanged.HasDelegate)
                    await AsteriskChanged.InvokeAsync(Asterisk);
            }
        }

        protected async void UpdateInformation(string asterisk)
        {
            if (string.IsNullOrWhiteSpace(asterisk)) // clearing
            {
                Value = null; 
                await InvokeAsync(StateHasChanged);
            } 
            else if (asterisk != Value?.Asterisk) // updating
            {
                var value = await APIClient.Telephony.Destination.FromAsterisk(asterisk, default);
                if (value == null) 
                {                    
                    value = new Destination()
                    {
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
