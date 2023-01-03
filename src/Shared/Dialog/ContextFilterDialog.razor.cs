using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Sales;
using System;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ContextFilterDialog : ComponentBase
    {
        [Inject]
        IContextView View { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        [CascadingParameter]
        [EditorRequired]
        MudDialogInstance MudDialog { get; set; } = default!;

        protected string? FilterText { get; set; }

        public async void OnClientSelect(IClient client)
        {
            await View.Update(client.Id);
        }

        public async void OnClearClicked(MouseEventArgs _)
        {
            await View.Update(Guid.Empty);
            MudDialog.Close();
        }

        void Close() => MudDialog.Close();
    }
}
