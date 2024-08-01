using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class MudNavLinkEnchanted : MudBaseSelectItem
    {
        protected string Classname =>
        new CssBuilder("mud-nav-item")
          .AddClass($"mud-ripple", !Ripple && !Disabled)
          .AddClass(Class)
          .Build();

        protected string LinkClassname =>
        new CssBuilder("mud-nav-link")
          .AddClass($"mud-nav-link-disabled", Disabled)
          .Build();

        protected string IconClassname
        {
            get
            {
                var basecss = new CssBuilder("mud-nav-link-icon")
                  .AddClass($"mud-nav-link-icon-default", IconColor == Color.Default)
                  .Build();

                return basecss + (IconAsAvatar ? " mud-icon-root shadow" : string.Empty);
            }
        }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors, default value uses the themes drawer icon color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [Parameter]
        [Category(CategoryTypes.NavMenu.ClickAction)]
        public string? Target { get; set; }
    
        public bool IconAsAvatar => string.IsNullOrEmpty(Icon) && !string.IsNullOrWhiteSpace(Title);

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? SubTitle { get; set; }

        protected string GetTitleClass => "mud-nav-link-text" + (!string.IsNullOrWhiteSpace(SubTitle) ? " mud-nav-link-text-extended" : string.Empty);

        protected string GetInitials()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(Title))
            {
                foreach (string s in Title.Split(" "))
                    if (s.Length > 3)
                        result += s[0];
            }
            return result;
        }

        [CascadingParameter] INavigationEventReceiver? NavigationEventReceiver { get; set; }

        protected Task HandleNavigation()
        {
            if (!Disabled && NavigationEventReceiver != null)
            {
                return NavigationEventReceiver.OnNavigation();
            }
            return Task.CompletedTask;
        }

        protected Dictionary<string, object?>? Attributes
        {
            get => Disabled ? null : new Dictionary<string, object?>()
            {
                { "href", Href },
                { "target", Target },
                { "rel", !string.IsNullOrWhiteSpace(Target) ? "noopener noreferrer" : string.Empty }
            };
        }
    }
}
