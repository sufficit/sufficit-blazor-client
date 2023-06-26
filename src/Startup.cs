using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.Client;
using Sufficit.Identity;
using Blazored.LocalStorage;
using MudBlazor;

namespace Sufficit.Blazor.Client
{
    public class Startup
    {
        private readonly WebAssemblyHostBuilder _builder;

        public Startup(WebAssemblyHostBuilder builder)
        {
            _builder = builder;
        }

        public WebAssemblyHost Build()
        {
            ConfigureServices(_builder.Services);
            return _builder.Build();
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Incluindo as configurações na injeção de dependencias
            services.TryAddSingleton<IConfiguration>(_builder.Configuration);

            // Configuração padrão para o gerenciamento de LOGS
            services.AddLogging();
            services.AddOptions();

            //services.AddHttpClient("BlazorHybrid", options => options.BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress));
            services.AddAuthenticationCore();
            services.AddSufficitAuthentication();
            services.AddBlazoredLocalStorage();
            
            services.AddScoped<IScrollManager, ScrollManager>();
            services.AddScoped<IAuthService, WasmAuthService>();
            services.AddScoped<ITokenProvider, WasmTokenProvider>();
            services.ConfigureCommonServices(_builder.Configuration);

            services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();

            return services;
        }
    }
}
