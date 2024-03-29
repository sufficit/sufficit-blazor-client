﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Client.Shared.Tables;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.EndPoints;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class Free : TelephonyBasePageComponent, IPage
    {
        public const string RouteParameter = "pages/telephony/did/free";

        protected override string Title => "DID Disponíveis";

        protected override string Description => "Rotas de entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [EditorRequired]
        protected DIDTable? Table { get; set; }

        protected EndPointFullResponse<Sufficit.Telephony.DirectInwardDialing>? Result { get; set; }

        protected async ValueTask<EndPointFullResponse<Sufficit.Telephony.DirectInwardDialing>> GetServerData(DIDSearchParameters parameters, CancellationToken cancellationToken)
        {
            parameters.ContextId = Guid.Empty;
            Result = await APIClient.Telephony.DID.FullSearch(parameters, cancellationToken);

            await InvokeAsync(StateHasChanged);
            return Result;
        }
    }
}
