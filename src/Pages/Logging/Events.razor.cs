using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
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
    public partial class Events : BasePageComponent
    {
        protected override string Title => "Events";

        protected override string Description => "Registro de eventos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [SupplyParameterFromQuery]
        [Parameter]
        public string? ClassName { get; set; }

        [SupplyParameterFromQuery(Name = "event.contextid")]
        [Parameter]
        public Guid? ContextId { get; set; }

        protected IEnumerable<JsonLog>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var parameters = new LogSearchParameters();
            parameters.ContextId = ContextId;
            if (!string.IsNullOrWhiteSpace(ClassName))
                parameters.ClassName = ClassName;

            Items = await APIClient.Logging.GetEventsWithContent(parameters, default);
            await InvokeAsync(StateHasChanged);
        }
    }
}
