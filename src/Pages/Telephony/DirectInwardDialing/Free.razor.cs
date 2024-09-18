using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Client.Shared.Tables;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.EndPoints;
using Sufficit.Telephony;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class Free : TelephonyBasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/did/free";

        public const string Title = "DID Disponíveis";
        protected override string Description => "Rotas de entrada";

        #endregion

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
