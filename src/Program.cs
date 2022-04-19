using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Para finalizar vamos construir o aplicativo para o usuário final
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            #region INCLUDING A SEPARATED FILE FOR EVENTS PANEL

            var jsonFile = "appsettings.EventsPanel.json";
            var http = new HttpClient()
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };

            using var response = await http.GetAsync(jsonFile);
            using var stream = await response.Content.ReadAsStreamAsync();

            builder.Configuration.AddJsonStream(stream);

            #endregion

            await new Startup(builder).Build().RunAsync();
        }
    }
}
