using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using Sufficit.Sales;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class DIDFreeOption : ComponentBase
    {
        [CascadingParameter]
        private Sufficit.Identity.UserPrincipal? Principal { get; set; } = default!;

        protected bool Visible
            => Principal?.IsInRole<SalesManagerRole>() ?? false;
    }
}
