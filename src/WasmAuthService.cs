using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
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

        [HttpGet]
        public Task<BlazorRemoteUser?> CurrentUser()
        {
            // not implemented yet, should store user locally
            throw new NotImplementedException();

            /*
            var responseMessage = await httpClient.GetAsync("api/authentication/currentuser");
            if (responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    return await responseMessage.Content.ReadFromJsonAsync<BlazorRemoteUser>();
                }
                catch(Exception ex) { 
                    //var content = await responseMessage.Content.ReadAsStringAsync();
                    //Console.WriteLine(content);
                    Console.WriteLine($"wasm current user error: {ex.Message}"); 
                }
            }
            return null;
            */
        }

        [HttpPost]
        public Task Login(string returnUrl)
        {
            // not implemented yet, should store user locally
            throw new NotImplementedException();

            /*
            var result = await httpClient.PostAsJsonAsync($"api/authentication/login", returnUrl);
            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"login: {response}");
            */
        }

        public async Task Logout()
        {
            var result = await httpClient.PostAsync("api/authentication/logout", null);
            result.EnsureSuccessStatusCode();
        }
    }
}
