using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.JSInterop;
using MudBlazor;
using Sufficit.Identity;
using Sufficit.Sales;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class AppBarDefault : ComponentBase
    {
        [Parameter]
        public bool SideBarExtended { get; set; }

        async void ToggleDrawer()
        {
            SideBarExtended = !SideBarExtended;
            if(SideBarExtendedChanged.HasDelegate)
                await SideBarExtendedChanged.InvokeAsync(SideBarExtended).ConfigureAwait(false);
        }

        [Parameter]
        public EventCallback<bool> SideBarExtendedChanged { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager uriHelper { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        protected async Task Refresh(MouseEventArgs e)
        {
            var parameters = new DialogParameters(); 
            parameters.Add("content", "Está salvo com sucesso.");
            DialogService.Show<StatusDialog>("Sucesso !", parameters);

                /*
                var alert = new SweetAlert()
                {
                    TimerProgressBar = true,
                    Timer = 8000,
                    Title = "Apagar cache",
                    Text = "Essa ação irá limpar todos os arquivos salvos (referentes a esta aplicação somente) em seu dispositivo.",
                    Icon = "question",
                    ShowDenyButton = true,
                    DenyButtonText = "Não",
                    ConfirmButtonText = "Continuar"
                };

                var Swal = UIService.SweetAlerts;
                var result = await Swal.Fire(alert);
                if (result != null)
                {
                    if (result.IsConfirmed)
                    {
                        await Reload();
                    }
                }
                */
            }

        protected async Task Reload()
        {
            await JSRuntime.InvokeVoidAsync("caches.delete", "blazor-resources-/");
            uriHelper.NavigateTo(uriHelper.Uri, forceLoad: true);
        }

        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        IAuthService Auth { get; set; } = default!;

        [CascadingParameter]
        protected UserPrincipal? User { get; set; }

        protected async Task LogOut(MouseEventArgs _)
        {
            await Auth.Logout();
            await InvokeAsync(StateHasChanged);
        }

        protected async Task LogIn(MouseEventArgs _)
        {
            string returnUrl = Navigation.Uri;
            await Auth.Login(returnUrl);
            await InvokeAsync(StateHasChanged);
        }
    }
}
