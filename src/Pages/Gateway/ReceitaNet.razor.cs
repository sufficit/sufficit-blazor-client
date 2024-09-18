using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Client.Components.Gateway.ReceitaNet;
using Sufficit.Blazor.Components;
using Sufficit.Telephony;
using System;

namespace Sufficit.Blazor.Client.Pages.Gateway
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class ReceitaNet : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/gateway/receitanet";

        public const string Title = "ReceitaNet";
        protected override string? Area => "Gateway";
        protected override string Description => "Gateway de integração com o ReceitaNet";

        #endregion

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? GatewayId { get; set; }

        protected ReceitaNetOptionsTable Table { get; set; } = default!;

        public static string GetLink(Guid? gatewayid)
        {
            var link = RouteParameter;
            if (gatewayid.HasValue)
                link += $"?{nameof(GatewayId).ToLower()}={gatewayid}";
            return link.TrimEnd('&');
        }
    }
}
