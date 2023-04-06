using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Client.Shared.Tables;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class Free : TelephonyBasePageComponent, IPage
    {
        public const string RouteParameter = "/pages/telephony/did/free";

        protected override string Title => "DID Disponíveis";

        protected override string Description => "Rotas de entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [EditorRequired]
        protected DIDTable? Table { get; set; }

        protected IEnumerable<Sufficit.Telephony.DirectInwardDialing>? Items { get; set; }

        protected async ValueTask<IEnumerable<Sufficit.Telephony.DirectInwardDialing>> GetServerData(DIDSearchParameters parameters, CancellationToken cancellationToken)
        {
            parameters.ContextId = Guid.Empty;
            //Items = await APIClient.Telephony.DID.Search(parameters, cancellationToken);
            await InvokeAsync(StateHasChanged);
            return Items;
        }
    }
}
