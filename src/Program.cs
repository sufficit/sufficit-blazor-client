using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            #region CONFIGURATIONS

            var http = new HttpClient()
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            };

            #region INCLUDING A SEPARATED FILE FOR EVENTS PANEL
            
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "appsettings.EventsPanel.json");                
                using var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStreamAsync();
                    if (json != null)
                    {
                        Console.WriteLine($"appsettings eventspanel from json: {request.RequestUri}");
                        builder.Configuration.AddJsonStream(json);
                    }
                }
            }

            #endregion

            {
                var environment = builder.HostEnvironment.Environment;
                var request = new HttpRequestMessage(HttpMethod.Get, $"appsettings.{environment}.json");
                using var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStreamAsync();
                    if (json != null)
                    {
                        Console.WriteLine($"appsettings for {environment} from json");
                        builder.Configuration.AddJsonStream(json);
                    }
                }
            }

            #endregion
            
            var app = new Startup(builder).Build();
            await app.RunAsync();
        }
    }
}
