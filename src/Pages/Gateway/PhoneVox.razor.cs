using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Client.Components.Gateway.PhoneVox;
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
    public partial class PhoneVox : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;

        public const string RouteParameter = "/pages/gateway/phonevox";

        public const string Title = "PhoneVox";
        protected override string? Area => "Gateway";
        protected override string Description => "Gateway de integração com a PhoneVox";

        #endregion

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? GatewayId { get; set; }

        protected PhoneVoxOptionsTable Table { get; set; } = default!;

        public static string GetLink (Guid? gatewayid)
        {
            var link = RouteParameter;
            if (gatewayid.HasValue)
                link += $"?{nameof(GatewayId).ToLower()}={gatewayid}";
            return link.TrimEnd('&');
        }
    }
}
