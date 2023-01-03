using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ConfirmDialog : ComponentBase
    {
        [CascadingParameter]
        [EditorRequired]
        protected MudDialogInstance? MudDialog { get; set; } = default!;

        [Parameter]
        public string? Question { get; set; }

        [Parameter]
        public string? Observation { get; set; }

        [Parameter]
        public Color Color { get; set; } = Color.Success;

        void Submit() => MudDialog?.Close(DialogResult.Ok(true));

        void Cancel() => MudDialog?.Cancel();
    }
}

