using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SufficitBlazorClient.Extensions;
using SufficitBlazorClient.Models;
using SufficitBlazorClient.Services;
using Microsoft.Extensions.Logging;
using Sufficit.APIClient.Extensions;

namespace SufficitBlazorClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Incluindo as configurações na injeção de dependencias
            services.AddSingleton(Configuration);

            // Configuração padrão para o gerenciamento de LOGS
            services.AddLogging(options => {

                // Busca as configurações no arquivo estático appsettings.json
                options.AddConfiguration(Configuration.GetSection("Logging"));
            });

            services.AddMudServices();
            services.AddOptions();
            services.AddBlazoredLocalStorage();

            services.AddSufficitAuthentication();
            services.AddSufficitEndPointsAPI();

            services.AddHttpClient("default");

            services.AddTransient<TelephonyService>();
            services.AddTransient<FetchWeatherForecastService>();

            // Incluindo serviço de auxilio a navegação
            services.AddSingleton<IBreadcrumbService, BreadcrumbService>();

            //services.AddSingleton<BlazorServerAuthStateCache>();
            services.AddHttpContextAccessor();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.AddJsSIP();
            services.AddBlazorUIMaterial();
        }
    }
}
