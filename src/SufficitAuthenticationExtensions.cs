using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sufficit.Identity.Configuration;
using Sufficit.Blazor;
using Sufficit.Blazor.Client.Services;
using System;

namespace Sufficit.Blazor.Client
{
    public static class SufficitAuthenticationExtensions
    {
        public static IServiceCollection AddSufficitAuthentication(this IServiceCollection services)
        {            
            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            
            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<OpenIDOptions>(options => configuration.GetSection(OpenIDOptions.SECTIONNAME));

            // importante para incluir propriedades extras ao usuario
            services.AddScoped<WasmAuthenticationService>();
            services.AddScoped<AuthenticationStateProvider, WasmAuthenticationService>((provider) => provider.GetRequiredService<WasmAuthenticationService>());
            services.AddScoped<IRemoteAuthenticationService<RemoteAuthenticationState>>((provider) => provider.GetRequiredService<WasmAuthenticationService>());
            services.AddScoped<IAccessTokenProvider>((provider) => provider.GetRequiredService<WasmAuthenticationService>());

            services.AddOidcAuthentication(options =>
            {
                // Capturando para uso local
                var oidOptions = configuration.GetSection(OpenIDOptions.SECTIONNAME).Get<OpenIDOptions>();
                if (oidOptions != null)
                {
                    oidOptions.Bind(options.ProviderOptions);

                    // oidOptions.GetClaimsFromUserInfoEndpoint = true;
                    // importante pois o nome padrão normalmente é o endereço completo da microsoft
                    // ex: https://micros...................role
                    options.UserOptions.NameClaim = "name";
                    options.UserOptions.RoleClaim = "role";

                    options.ProviderOptions.PostLogoutRedirectUri = $"./{AuthenticationController.RouteParameter}/logout-callback";
                    options.ProviderOptions.RedirectUri = $"./{AuthenticationController.RouteParameter}/login-callback";
                }
            });

            return services;
        }
    }
}
