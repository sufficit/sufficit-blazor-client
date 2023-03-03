using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Client.Models;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class IDestinationItemTemplate : ComponentBase
    {
        [Parameter]
        [EditorRequired]
        public IDestination Destination { get; set; } = default!;

        protected string Icon
            => Destination.GetIcon();
    }
}
