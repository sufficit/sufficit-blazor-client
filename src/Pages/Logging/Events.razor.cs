using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Logging;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Logging
{
    [Authorize(Roles = "manager")]
    public partial class Events : BasePageComponent, IPage
    {
        public const string RouteParameter = "pages/logging/events";

        protected override string Title => "Events";

        protected override string Description => "Registro de eventos";

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
