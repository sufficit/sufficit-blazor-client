using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudButtonEnchanted : MudButton
    {
        protected bool disabled => Disabled || IsLoading;

        protected bool IsLoading;

        protected new async Task OnClickHandler(MouseEventArgs ev)
        {            
            if (!GetDisabledState())
            {
                IsLoading = true;
                await InvokeAsync(StateHasChanged);

                // normal handling
                await base.OnClickHandler(ev);
                IsLoading = false;
            }
        }
    }
}
