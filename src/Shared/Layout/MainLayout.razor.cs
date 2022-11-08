using MudBlazor.ThemeManager;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Identity.Client;
using Microsoft.AspNetCore.Http;
using Sufficit.Identity;

namespace Sufficit.Blazor.Client.Shared.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        public bool SideBarExtended { get; set; } = default!;

        private MudThemeProvider? ThemeProvider { get; set; } = default!;

        protected void ToggleDrawer()
        {
            SideBarExtended = !SideBarExtended;
        }


        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _themeManager.Theme = MyCustomTheme;
        }

        protected CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        MudTheme MyCustomTheme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = Colors.Red.Darken1,
                Secondary = Colors.Green.Accent4,
                AppbarBackground = Colors.Shades.White
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
