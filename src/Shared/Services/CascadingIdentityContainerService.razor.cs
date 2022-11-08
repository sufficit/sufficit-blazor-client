using Microsoft.AspNetCore.Components;
using System;

namespace Sufficit.Blazor.Client.Shared.Services
{
    public partial class CascadingIdentityContainerService : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}