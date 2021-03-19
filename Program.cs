using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using MudBlazor.Services;

using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;
using SufficitBlazorClient.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SufficitBlazorClient
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddMudServices();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            #region TEST OF AUTHENTICATION
            /*
            builder.Services.AddSingleton<ApplicationAuthenticationState>();

            builder.Services.AddOidcAuthentication<ApplicationAuthenticationState, CustomUserAccount>(options =>
            {
                builder.Configuration.Bind("Local", options.ProviderOptions);
            });
            */

            builder.Services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
            builder.Services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
            builder.Services.AddTransient<AuthorizedHandler>();
            builder.Services.AddHttpClient("default", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient("authorizedClient").AddHttpMessageHandler<AuthorizedHandler>();


            builder.Services.AddScoped<SufficitEndPointsAuthorizationMessageHandler>();
            builder.Services.AddHttpClient("ServerAPI",
                client => client.BaseAddress = new Uri("https://apps.sufficit.com.br:26503"))
            .AddHttpMessageHandler<SufficitEndPointsAuthorizationMessageHandler>();

            // Usando o SufficitBlazorServer como padrão para as requisições
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
            builder.Services.AddTransient<FetchWeatherForecastService>();

            #endregion
            #region USE OF SUFFICIT GRAPHQL TO DATA

            //builder.Services.addwe

            #endregion

            // Incluindo serviço para armazenar informações locais em cada cliente
            // Singleton, significa que será uma única instancia para cada cliente
            builder.Services.AddSingleton<WebAssemblyLocalStorageService>();


            // Para finalizar vamos construir o aplicativo para o usuário final
            builder.RootComponents.Add<App>("#app");

            await builder.Build().RunAsync();
        }
    }
}
