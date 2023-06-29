using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Sufficit.Identity;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class AppBarDefault : ComponentBase, IDisposable
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
        protected IDialogService DialogService { get; set; } = default!;

        [Inject]
        protected IScrollListener ScrollListener { get; set; } = default!;

        protected void Refresh(MouseEventArgs e)
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
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
        }

        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        IAuthService Authentication { get; set; } = default!;

        [CascadingParameter]
        protected UserPrincipal? User { get; set; }

        protected async Task LogOut(MouseEventArgs _)
        {
            await Authentication.Logout();
            await InvokeAsync(StateHasChanged);
        }

        protected async Task LogIn(MouseEventArgs _)
        {
            string returnUrl = Navigation.Uri;
            await Authentication.Login(returnUrl);
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            ScrollListener.OnScroll += OnScrollEvent;
        }

        public void Dispose()
        {
            if (ScrollListener != null)
            {
                ScrollListener.OnScroll -= OnScrollEvent;
                ScrollListener.Dispose();
            }
        }

        protected string? AppBarStyle { get; set; }
        double lastMaxPosition = 0;
        double lastFixedPosition = 0;
        double lastScrollTop = 0;

        private async void OnScrollEvent(object? sender, ScrollEventArgs e)
        {
            int scrollPosition = e.NodeName == "#document"
                ? (int)(e.FirstChildBoundingClientRect.Top * -1) : (int)e.ScrollTop;
                       
            double opacity = 0;
            if (scrollPosition < lastScrollTop)
            {
                lastFixedPosition = scrollPosition;
                opacity = - (scrollPosition - lastMaxPosition) / 100;
            }
            else
            {
                lastMaxPosition = scrollPosition;
                opacity = 1 - (scrollPosition - lastFixedPosition) / 100;
            }

            // Normalizing opacity to css format
            if (opacity < 0) opacity = 0;
            else if (opacity > 1) opacity = 1;

            AppBarStyle = $"opacity: {opacity}; ";            
            if (opacity > 0)
                AppBarStyle += "display: flex;";
            else
                AppBarStyle += "display: none;";

            // Console.WriteLine($"scroll: {e.FirstChildBoundingClientRect.Top} => {scrollPosition} => {AppBarStyle}");
            lastScrollTop = scrollPosition;
            await InvokeAsync(StateHasChanged);
        }
    }
}
