using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sufficit.Identity.Configuration;

namespace Sufficit.Blazor.Client
{
    public static class SufficitOpenIDOptionsExtensions
    {
        /// <summary>
        /// Realiza a configuração da autenticação via Open ID da Sufficit
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        public static void Bind(this OpenIDOptions source, OidcProviderOptions options)
        {
            options.Authority = source.Authority;
            options.ClientId = source.ClientId;
            options.ResponseType = source.ResponseType;

            if (source.Scopes != null)
            {
                options.DefaultScopes.Clear();
                foreach (var scope in source.Scopes)
                    options.DefaultScopes.Add(scope);
            }
        }
    }
}
