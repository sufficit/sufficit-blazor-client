using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material;
using Sufficit.Blazor.UI.Material.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class ContactAvatar
    {
        [Parameter]
        public Guid? IDReference { get; set; }

        [Parameter]
        public ComponentSize Size { get; set; }

        protected string GetSource()
        {
            return $"https://www.sufficit.com.br/Relacionamento/Avatar.ashx?IDContexto={ IDReference }";
        }
    }
}
