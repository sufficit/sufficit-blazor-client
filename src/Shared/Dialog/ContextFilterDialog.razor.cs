using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Sales;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ContextFilterDialog : ComponentBase
    {
        [Inject]
        IContextView View { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        [Inject]
        protected BlazorContextRuntime BCRuntime { get; set; } = default!;

        [CascadingParameter]
        [EditorRequired]
        MudDialogInstance MudDialog { get; set; } = default!;

        [EditorRequired]
        protected MudTextField<string?> TextField { get; set; } = default!;

        protected string? FilterText { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            { // somehow input.AutoFocus is not working, so its a trick
                var er = TextField.InputReference.ElementReference;
                await BCRuntime.Focus(er);
            }
        }

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
