using Microsoft.AspNetCore.Components;
using MudBlazor;
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
        public Size Size { get; set; }

        protected string SourceUrl => $"https://www.sufficit.com.br/Relacionamento/Avatar.ashx?IDContexto={ IDReference }";
    }
}
