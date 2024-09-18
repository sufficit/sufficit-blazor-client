using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class DashBoard : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/gateway";
        
        public const string Icon = Icons.Material.Filled.Extension;        
        protected override string? Area => "Gateway";
        public const string Title = "Gateways";
        protected override string Description => "Modulos de Integrações";

        #endregion
    }
}
