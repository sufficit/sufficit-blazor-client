using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sufficit.Blazor.Client.Models;
using Sufficit.Blazor.Client.Services;
using Microsoft.Extensions.Logging;
using Sufficit.Client;
using Sufficit.Identity.Client;
using Sufficit.Blazor.UI.Material.Extensions;
using Sufficit.Telephony.EventsPanel;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Sufficit.Blazor;
using Microsoft.AspNetCore.Components.Authorization;

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
            services.AddSingleton<IConfiguration>(_builder.Configuration);

            // Configuração padrão para o gerenciamento de LOGS
            services.AddLogging(options => {

                // Busca as configurações no arquivo estático appsettings.json
                options.AddConfiguration(_builder.Configuration.GetSection("Logging"));
            });

            services.AddOptions();
            services.ConfigureCommonServices();

            services.AddHttpClient("BlazorHybrid", options => options.BaseAddress = new Uri(_builder.HostEnvironment.BaseAddress));
            services.AddSingleton<IAuthService, WasmAuthService>();

            services.AddAuthorizationCore();

            services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();

            //services.AddSufficitAuthentication();
            return services;
        }
    }
}
