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

        public void OnClientSelect(IClient client)
        {
            View.Update(client.Id);
        }

        public void OnClearClicked(MouseEventArgs _)
        {
            View.Update(null);
            MudDialog.Close();
        }

        void Close() => MudDialog.Close();
    }
}
