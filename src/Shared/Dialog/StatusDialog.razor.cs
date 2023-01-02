using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class StatusDialog : ComponentBase
    {
        [Parameter]
        public string Content { get; set; } = default!;

        [Parameter]
        public Exception? Ex { get; set; }

        [CascadingParameter]
        [EditorRequired]
        MudDialogInstance MudDialog { get; set; } = default!;

        void Submit() => MudDialog.Close(DialogResult.Ok(true));
    }
}

