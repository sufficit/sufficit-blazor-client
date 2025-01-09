using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using Sufficit.Blazor.Services;
using Sufficit.Blazor.Client.Services;

namespace Sufficit.Blazor.Client
{
    public class Startup
    {
        private readonly WebAssemblyHostBuilder _builder;

        public Startup (WebAssemblyHostBuilder builder)
        {
            _builder = builder;
        }

        public WebAssemblyHost Build()
        {
            ConfigureServices(_builder.Services);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

            return _builder.Build();
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {            
            // Incluindo as configurações na injeção de dependencias
            services.TryAddSingleton<IConfiguration>(_builder.Configuration);

            // Configuração padrão para o gerenciamento de LOGS
            services.AddLogging();
            services.AddOptions();

            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();
            services.AddAuthenticationStateDeserialization();

            services.ConfigureCommonServices(_builder.Configuration);

            // Add device-specific services used by the BlazorHybrid.Shared project
            services.AddSingleton<IFormFactor, FormFactor>();

            return services;
        }
    }
}
