using Microsoft.AspNetCore.Components;
using Sufficit.Client;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class APIStatusIconMenuItem : ComponentBase, IDisposable
    {
        [Inject]
        protected APIClientService APIClient { get; set; } = default!;

        protected string Tooltip => $"({APIClient.HealthChecked.ToString("HH:mm:ss")}) API não disponível";

        protected bool Visible => APIClient.HealthChecked > DateTime.MinValue && !APIClient.Available;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await APIClient.GetStatus();

            APIClient.OnChanged -= OnControlChanged;
            APIClient.OnChanged += OnControlChanged;

        }

        private async void OnControlChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            APIClient.OnChanged -= OnControlChanged;
        }
    }
}
