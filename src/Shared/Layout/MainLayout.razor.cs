using MudBlazor.ThemeManager;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Identity.Client;
using Microsoft.AspNetCore.Http;
using Sufficit.Identity;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Blazor.Components;

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

        [CascadingParameter(Name = "PageType")]
        public Type? PageType { get; set; }

        protected string GetPageTitle()
        {
            string title = "Sufficit";
            if (PageType == null)
                title += " - Não encontrado";
            else // if (PageType.IsAssignableTo(typeof(IPage))) 
            {
                var page = PageType.GetField(nameof(IPage.Title))?.GetValue(null)?.ToString();
                if (!string.IsNullOrWhiteSpace(page))
                    title += $" - {page}";
            }
            return title;
        }


        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            MyCustomTheme.Shadows.Elevation[3] = "0px 2px 4px -1px rgba(200,200,200,0.2),0px 4px 5px 0px rgba(200,200,200,0.14),0px 1px 10px 0px rgba(200,200,200,0.12);";
            _themeManager.Theme = MyCustomTheme;
        }

        protected CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        MudTheme MyCustomTheme = new MudTheme()
        {            
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Red.Darken1,
                Secondary = Colors.Green.Accent4,
                AppbarBackground = Colors.Shades.White,
                Info = Colors.Blue.Lighten3
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
            },            
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
