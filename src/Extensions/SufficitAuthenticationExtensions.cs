﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity.Configuration;
using SufficitBlazorClient.Models.Identity;
using SufficitBlazorClient.Services;
using System;

namespace SufficitBlazorClient.Extensions
{
    public static class SufficitAuthenticationExtensions
    {
        public static IServiceCollection AddSufficitAuthentication(this IServiceCollection services)
        {
            IConfiguration? configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null) throw new ArgumentNullException("configuration");

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<OpenIDOptions>(options => configuration.GetSection(OpenIDOptions.SectionName).Bind(options));

            // Capturando para uso local
            var oidOptions = configuration.GetSection(OpenIDOptions.SectionName).Get<OpenIDOptions>();
            
            // Usado na página /authentication
            services.AddScoped<RemoteAuthenticationState, CustomRemoteAuthenticationState>();

            services.AddScoped<RemoteUserAccount, CustomRemoteUserAccount>();

            services.AddScoped<CustomAccountClaimsPrincipalFactory>();
            services.AddScoped<AccountClaimsPrincipalFactory<CustomRemoteUserAccount>, CustomAccountClaimsPrincipalFactory>();

            services.AddAuthorizationCore(); 
            services.AddOidcAuthentication<RemoteAuthenticationState, CustomRemoteUserAccount>(options =>
            {
                oidOptions.Bind(options.ProviderOptions);

                // importante pois o nome padrão normalmente é o endereço completo da microsoft
                // ex: https://micros...................role
                options.UserOptions.RoleClaim = "role";
            })
                .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, CustomRemoteUserAccount, CustomAccountClaimsPrincipalFactory>();

            // importante para incluir propriedades extras ao usuario
            services.AddScoped<AuthenticationStateProvider, CustomRemoteAuthenticationService>();

            return services;
        }
    }
}
