using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.Telephony.JsSIP;
using System;

namespace SufficitBlazorClient.Extensions
{
    public static class SufficitJsSIPExtensions
    {
        public static IServiceCollection AddJsSIP(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null) throw new ArgumentNullException("configuration");

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<JsSIPOptions>(options => configuration.GetSection(JsSIPOptions.SectionName).Bind(options));

            // Incluindo serviço de softphone em javascript
            services.AddSingleton<JsSIPSessions>();
            services.AddSingleton<JsSIPService>();
            services.TryAddSingleton(sp => sp.GetRequiredService<JsSIPService>());

            return services;
        }
    }
}
