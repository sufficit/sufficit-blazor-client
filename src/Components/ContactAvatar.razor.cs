using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material.Components;
using System;

namespace Sufficit.Blazor.Client.Components
{
    public partial class ContactAvatar : Avatar
    {
        [Parameter]
        public Guid? IDReference { get; set; }

        protected override string GetSource()
        {
            return $"https://www.sufficit.com.br/Relacionamento/Avatar.ashx?IDContexto={ IDReference }";
        }

        // hiding base source parameter
        public new string Source;
    }
}
