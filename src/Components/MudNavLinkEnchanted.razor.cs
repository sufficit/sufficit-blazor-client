using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Interfaces;
using Sufficit.Blazor.UI.Material;
using Sufficit.Blazor.UI.Material.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class MudNavLinkEnchanted : MudNavLink
    {
        public bool IconAsAvatar => string.IsNullOrEmpty(Icon) && !string.IsNullOrWhiteSpace(Title);

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string? SubTitle { get; set; }

        public new string IconClassname => base.IconClassname + (IconAsAvatar ? " mud-icon-root shadow" : string.Empty);

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
    }
}
