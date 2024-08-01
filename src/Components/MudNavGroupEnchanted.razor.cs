using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class MudNavGroupEnchanted : MudNavGroup
    {
        [Parameter]
        public bool IconAsAvatar { get; set; }

        [Parameter]
        public string? SubTitle { get; set; }

        public new string IconClassname => base.IconClassname + (IconAsAvatar ? " mud-icon-root shadow" : string.Empty);

        protected string GetTitleClass => "mud-nav-link-text" + (!string.IsNullOrWhiteSpace(SubTitle) ? " mud-nav-link-text-extended" : string.Empty);

        protected string GetInitials()
        {
            if (string.IsNullOrWhiteSpace(Title))
                return string.Empty;

            string result = string.Empty;
            foreach (string s in Title.Split(" "))
                if (s.Length > 3)
                    result += s[0];
            return result;
        }

        private async Task ExpandedToggleAsync()
        {
            Expanded = !Expanded;
            await InvokeAsync(StateHasChanged);
        }
    }
}
