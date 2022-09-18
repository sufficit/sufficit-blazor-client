using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Identity.Client;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ClientMissing : ComponentBase
    {
        [Inject]
        IDialogService Dialog { get; set; } = default!;

        [CascadingParameter]
        public MudTheme? Theme { get; set; }

        protected void ChooseClicked(MouseEventArgs _)
        {
            Dialog.Show<ContextFilterDialog>();
        }
    }
}
