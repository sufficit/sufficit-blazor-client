using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sufficit.Identity;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class WasmAuthService : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly HttpContextAccessor _accessor;

        public WasmAuthService(IHttpClientFactory factory, HttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
            httpClient = factory.CreateClient("BlazorHybrid");
        }

        public Task<BlazorRemoteUser> CurrentUser()
        {
            return httpClient.GetFromJsonAsync<BlazorRemoteUser>("api/authentication/currentuser")!;
        }

        public async Task Login(string returnUrl)
        {
            var result = await httpClient.PostAsJsonAsync($"api/authentication/login", returnUrl);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"login: {response}");
        }

        public async Task Logout()
        {
            var result = await httpClient.PostAsync("api/authentication/logout", null);

            result.EnsureSuccessStatusCode();
        }
    }
}
