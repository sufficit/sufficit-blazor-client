using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Storage;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.MusicOnHold
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class DashBoard : TelephonyBasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/musiconhold/dashboard";

        public const string? Icon = Icons.Material.Filled.QueueMusic;
        public const string Title = "Música de espera";

        #endregion

        public static string GetLink(Guid? classid)
            => RouteParameter + GetQuery(classid);

        public static string GetQuery(Guid? classid)
        {
            var query = "?";
            if (classid.HasValue)
                query += $"classid={classid.Value}";
            return query.TrimEnd('&');
        }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? ClassId { get; set; }

        [Inject]
        private IContextView ContextView { get; set; } = default!;

    }
}
