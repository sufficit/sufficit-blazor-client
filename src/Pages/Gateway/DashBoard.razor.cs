using MudBlazor;
using Sufficit.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    public partial class DashBoard : BasePageComponent
    {
        public const string RouteParameter = "/pages/gateway";
        
        public const string Icon = Icons.Material.Filled.Extension;
        
        protected override string? Area => "Gateway";

        protected override string Title => "Gateways";

        protected override string Description => "Modulos de Integrações";
    }
}
