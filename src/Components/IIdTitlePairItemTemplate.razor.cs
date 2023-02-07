using Microsoft.AspNetCore.Components;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class IIdTitlePairItemTemplate : ComponentBase
    {
        [Parameter]
        [EditorRequired]
        public IIdTitlePair Item { get; set; } = default!;
    }
}
