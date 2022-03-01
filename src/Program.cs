using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using System.Threading.Tasks;

namespace SufficitBlazorClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Para finalizar vamos construir o aplicativo para o usuário final
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            new Startup(builder.Configuration).ConfigureServices(builder.Services);           

            await builder.Build().RunAsync();
        }
    }
}
