using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudButtonEnchanted : MudButton
    {
        protected bool _disabled => Disabled || IsLoading;

        protected bool IsLoading;

        protected new async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            IsLoading = true;
            await OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
            Activateable?.Activate(this, ev);
            IsLoading = false;
        }
    }
}
