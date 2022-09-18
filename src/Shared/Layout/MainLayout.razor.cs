using MudBlazor.ThemeManager;
using MudBlazor;
using Sufficit.Blazor.UI.Material.Services;
using Sufficit.Blazor.UI.Material;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;

namespace Sufficit.Blazor.Client.Shared.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        public bool SideBarExtended { get; set; }

        private MudThemeProvider? ThemeProvider { get; set; } = default!;

        void ToggleDrawer()
        {
            SideBarExtended = !SideBarExtended;
        }

        [Inject]
        public BlazorUIMaterialService UIService { get; internal set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        public TextSearchControl? TextSearch { get; internal set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TextSearch = new TextSearchControl(Navigation);
            _themeManager.Theme = MyCustomTheme;
        }

        protected CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UIService.EnsureIsLoaded(CancellationTokenSource.Token);
            }
        }

        MudTheme MyCustomTheme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = Colors.Red.Darken1,
                Secondary = Colors.Green.Accent4,
                AppbarBackground = Colors.Shades.White,
            },
            Typography = new Typography()
            {

                Default = new Default()
                {
                    FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                },
                Button = new MudBlazor.Button
                {
                    FontSize = ".75rem",
                    FontWeight = 700,
                    LineHeight = 2.25,
                },
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "0.5rem",
                DrawerMiniWidthLeft = "120px",
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "300px",
            }
        };

        #region THEME MANAGER

        private ThemeManagerTheme _themeManager = new ThemeManagerTheme();
        public bool _themeManagerOpen = false;

        void OpenThemeManager(bool value)
        {
            _themeManagerOpen = value;
        }

        void UpdateTheme(ThemeManagerTheme value)
        {
            _themeManager = value;
            StateHasChanged();
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
        }

        #endregion
    }
}
