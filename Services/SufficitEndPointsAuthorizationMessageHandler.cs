using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class SufficitEndPointsAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public SufficitEndPointsAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { navigationManager.BaseUri },
                scopes: new[] { "read" });
        }
    }
}
