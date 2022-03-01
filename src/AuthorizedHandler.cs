using SufficitBlazorClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SufficitBlazorClient
{
    public class AuthorizedHandler : DelegatingHandler
    {
        private readonly HostAuthenticationStateProvider _authenticationStateProvider;

        public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage = null;
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                
                if (!authState.User.Identity.IsAuthenticated)
                {
                    // if user is not authenticated, immediately set response status to 401 Unauthorized
                    responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                else
                {
                    responseMessage = await base.SendAsync(request, cancellationToken);
                }

                if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // if server returned 401 Unauthorized, redirect to login page
                    _authenticationStateProvider.SignIn();
                }
            }
            catch
            {

            }
            return responseMessage;
        }
    }
}
