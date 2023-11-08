using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Logging
{
    [Authorize(Roles = "manager")]
    public partial class Events : BasePageComponent, IPage
    {
        public const string RouteParameter = "pages/logging/events";

        public const string? Icon = MudBlazor.Icons.Material.Filled.Event;

        protected override string Title => "Eventos";

        protected override string Description => "Registro de eventos";

        protected override string? Area => "Logging";

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

        public static string GetLink(Guid? contextid, string? reference, string? @class)
        {
            var link = RouteParameter + "?";
            if (contextid.HasValue)
                link += $"{nameof(EventContextId)}={contextid}&";
            if (!string.IsNullOrWhiteSpace(reference))
                link += $"{nameof(Reference)}={reference}&";
            if (!string.IsNullOrWhiteSpace(@class))
                link += $"{nameof(ClassName)}={@class}&";
            return link.TrimEnd('&');
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
