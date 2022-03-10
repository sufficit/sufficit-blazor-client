using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Sufficit.Blazor.UI.Material;
using Sufficit.Client;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Pages.Identity
{
    [Authorize]
    public partial class Policies : ComponentBase
    {
        [Inject]
        private ILogger<Policies> Logger { get; set; }

        [Inject]
        private IdentityClientService Identity { get; set; }

        [Inject]
        private IHttpClientFactory Factory { get; set; }

        [Inject]
        IAccessTokenProvider TokenProvider { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject] APIClientService Service { get; set; }

        private HttpClient http { get; set; }
        private ClaimsPrincipal User { get; set; }
        private string Response { get; set; }
        private bool Success { get; set; }
        private string Status { get; set; }
        private IEnumerable<IDirective> Directives { get; set; } = new IDirective[] { };

        private GetUsersResponse UsersResponse { get; set; }
        private string UsersMessage { get; set; }
        private Sufficit.Identity.Client.User UserSelected;

        private GetUserClaimsResponse UserClaimsResponse { get; set; }
        private string UserClaimsMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (http == null)
            {
                http = Factory.CreateClient();
                http.BaseAddress = new Uri("https://identity.sufficit.com.br:26602");
            }

            await Request();

            //Directives = 
        }

        protected async Task<IEnumerable<IListItem>> GetItems(string filter, int results, CancellationToken cancellationToken)
        {
            var collection = new List<IListItem>();
            foreach (var item in await Service.Identity.GetDirectives(filter, results, cancellationToken))
            {
                var listItem = new ListItem();
                listItem.Value = item.ID.ToString();
                listItem.Description = item.Name;
                listItem.Title = item.Description;
                listItem.Anchor = item.Key;
                collection.Add(listItem);
            }
            return collection;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Status = (await Identity.Health())?.Status;
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task Request()
        {
            string endpoint = string.Format("/api/Users/{0}/Claims", "095132cd-b1c4-4043-ae87-0a59cf2e0569");
            try
            {
                var accessTokenResult = await TokenProvider.RequestAccessToken(new AccessTokenRequestOptions() { Scopes = new string[] { "skoruba_identity_admin_api" } });
                if (accessTokenResult.TryGetToken(out AccessToken token))
                {
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = HttpMethod.Get;
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                    request.RequestUri = new Uri(endpoint, UriKind.Relative);
                    var response = await http.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    Response = await response.Content.ReadAsStringAsync();
                    Success = true;
                    StateHasChanged();
                }
                else
                {
                    Response = "não foi possível recuperar o token de autorização";
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                Response = exception.Message;
                Logger.LogError("erro ao consultar api redirecionando para token", exception);
                exception.Redirect();
            }
            catch (Exception ex)
            {
                Response = ex.Message;
                Logger.LogError($"erro ao consultar api :: { ex.Message }");
            }
        }

        protected async Task ValueChanged(ChangeEventArgs args)
        {
            var searchText = args.Value.ToString();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length > 3)
            {
                Logger.LogInformation("GetUsersAsync");
                UsersResponse = await Identity.Users.GetUsersAsync(args.Value.ToString());
                if (UsersResponse == null)
                {
                    UsersResponse = null;
                    UsersMessage = "Problema na consulta";
                }
                else if (UsersResponse.Users == null || !UsersResponse.Users.Any())
                {
                    UsersResponse = null;
                    UsersMessage = "Nenhum resultado encontrado";
                }
            }
            else
            {
                UsersResponse = null;
                UsersMessage = "Mínimo de 4 caracteres para consultar";
            }
        }

        protected async Task SelectUser(User selected)
        {
            UserSelected = selected;
            await GetUserClaims(UserSelected);
        }

        protected async Task GetUserClaims(User selected)
        {
            Logger.LogInformation("GetUserClaims");
            UserClaimsResponse = await Identity.Users.GetUserClaimsAsync(selected.ID);
            if (UserClaimsResponse == null)
            {
                UserClaimsResponse = null;
                UserClaimsMessage = "Problema na consulta";
            }
            else if (UserClaimsResponse.Claims == null || !UserClaimsResponse.Claims.Any())
            {
                UserClaimsResponse = null;
                UserClaimsMessage = "Nenhum resultado encontrado";
            }
        }
    }
}
