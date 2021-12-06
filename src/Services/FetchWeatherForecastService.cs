using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit;
using Sufficit.APIClient;
using Sufficit.EndPoints.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class FetchWeatherForecastService
    {
        private readonly ILogger _logger;
        private readonly APIClientService _apiClient;
        private readonly HttpClient _publicApiClient;
        private readonly HttpClient _protectedApiClient;    

        public FetchWeatherForecastService(APIClientService client, ILogger<FetchWeatherForecastService> logger, IHttpClientFactory clientFactory, NavigationManager navigationManager)
        {
            _apiClient = client;
            _logger = logger;
            _publicApiClient = clientFactory.CreateClient();
            _protectedApiClient = clientFactory.CreateClient("authorizedClient");
            _publicApiClient.BaseAddress = _protectedApiClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }

        public async Task<WeatherForecast[]> GetPublicWeatherForeacast()
        {
            return await _publicApiClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");
        }

        public async Task<WeatherForecast[]> GetProtectedWeatherForeacast()
        {
            return await _protectedApiClient.GetFromJsonAsync<WeatherForecast[]>("/api/WeatherForecast");
        }

        public async Task<WeatherForecast[]> GetEndpointWeatherForeacast()
        => await _apiClient.WeatherForeacast();
    }
}
