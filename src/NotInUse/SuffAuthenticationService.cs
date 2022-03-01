using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SufficitBlazorClient.Models;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class SuffAuthenticationService : RemoteAuthenticationService<RemoteAuthenticationState, RemoteUserAccount, RemoteAuthenticationUserOptions>
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public CustomRemoteUserAccount User { get; private set; }

        public SuffAuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            Microsoft.JSInterop.IJSRuntime iJSRuntime,
            Microsoft.Extensions.Options.IOptionsSnapshot<RemoteAuthenticationOptions<RemoteAuthenticationUserOptions>> options,
            AccountClaimsPrincipalFactory<RemoteUserAccount> factory
        ) :base(iJSRuntime, options, navigationManager, factory)
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        } 

        public async Task Initialize()
        {
            User = await _localStorageService.GetItemAsync<CustomRemoteUserAccount>("user");
        }

        public async Task Login(string username, string password)
        {
            User = await _httpService.Post<CustomRemoteUserAccount>("/users/authenticate", new { username, password });
            await _localStorageService.SetItemAsync("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItemAsync("user");
            _navigationManager.NavigateTo("login");
        }
    }
}
