using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Sufficit.Telephony.JsSIP;
using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class Keypad : ComponentBase
    {
        [Inject]
        protected BlazorContextRuntime BCRuntime { get; set; } = default!;

        [Parameter]
        public bool CanOriginate { get; set; } = true;

        [Parameter]
        public EventCallback<string> CallStart { get; set; }

        #region DESTINATION

        /// <summary>
        ///     Destination for dialing
        /// </summary>
        [Parameter]
        public string? Destination { get; set; }

        public EventCallback<string?> DestinationChanged { get; set; }

        /// <summary>
        ///     Internal dial destination
        /// </summary>
        protected string? destination { get; set; }

        #endregion

        [EditorRequired]
        protected MudInput<string> DestinationInput { get; set; } = default!;

        protected bool CanStartCall
            => CanOriginate && !string.IsNullOrWhiteSpace(destination);

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrWhiteSpace(Destination))            
                destination = Destination;            
        }

        protected void ShiftDestination()
        {
            if (string.IsNullOrWhiteSpace(destination))
                return;

            SetDestination(destination.Remove(destination.Length - 1, 1));
        }

        protected void AddToDestination(char value)
            => SetDestination(destination + value);

        protected void AddToDestination(int value)
            => SetDestination(destination + value);

        protected void AddToDestination(string value)
            => SetDestination(destination + value);

        protected async void SetDestination(string? value)
        {
            if (destination == value)
                return;
            
            destination = value;
            await DestinationChanged.InvokeAsync(destination);
            await InvokeAsync(StateHasChanged);
        }

        private async Task InternalCallStart()
        {
            await CallStart.InvokeAsync(destination);
        }

        private async void HandleKeyDown(KeyboardEventArgs e)
        {
            var focused = await BCRuntime.IsFocused(DestinationInput!.ElementReference);                        
            switch (e.Key)
            {
                case "Backspace": if (!focused) ShiftDestination(); break;
                case "Enter": if (CanStartCall) { await InternalCallStart(); } break;                  
                default:
                    {
                        
                        if (!focused && e.Key.Length == 1 && char.IsAsciiLetterOrDigit(e.Key[0]))
                            AddToDestination(e.Key);
                        
                        break;
                    }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await DestinationInput.FocusAsync();            
        }
    }
}
