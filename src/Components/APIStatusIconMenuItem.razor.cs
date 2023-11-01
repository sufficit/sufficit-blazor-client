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

        protected override async Task OnInitializedAsync()
        {
            APIClient.OnChanged += OnControlChanged;
            _ = await APIClient.Health();
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
