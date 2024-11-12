using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Logging
{
    [Authorize(Roles = ManagerRole.NormalizedName)]
    public partial class Events : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/logging/events";

        public const string? Icon = MudBlazor.Icons.Material.Filled.Event;
        public const string Title = "Eventos";
        protected override string Description => "Registro de eventos";
        protected override string? Area => "Logging";

        #endregion

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [SupplyParameterFromQuery]
        [Parameter]
        public string? ClassName { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public string? Reference { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public Guid? EventContextId { get; set; }

        public static string GetLink (Guid? contextid, string? reference, string? @class)
            => RouteParameter + GetQuery(contextid, reference, @class);

        public static string GetQuery (Guid? contextid, string? reference, string? @class)
        {
            var query = "?";
            if (contextid.HasValue)
                query += $"{nameof(EventContextId)}={contextid}&";
            if (!string.IsNullOrWhiteSpace(reference))
                query += $"{nameof(Reference)}={reference}&";
            if (!string.IsNullOrWhiteSpace(@class))
                query += $"{nameof(ClassName)}={@class}&";

            return query.TrimEnd('&');
        }

        protected IEnumerable<JsonLog>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var parameters = new LogSearchParameters();
            parameters.ContextId = EventContextId;
            parameters.Reference = Reference;

            if (!string.IsNullOrWhiteSpace(ClassName))
                parameters.ClassName = ClassName;

            Items = await APIClient.Logging.GetEventsWithContent(parameters, default);
            await InvokeAsync(StateHasChanged);
        }
    }
}
