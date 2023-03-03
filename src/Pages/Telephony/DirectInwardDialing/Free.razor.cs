using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class Free : TelephonyBasePageComponent, IPage
    {
        public const string RouteParameter = "/pages/telephony/did/free";

        protected override string Title => "DID Disponíveis";

        protected override string Description => "Rotas de entrada";                 
    }
}
