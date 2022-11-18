using Sufficit.Blazor.Client.Models;
using System.Collections.Generic;

namespace Sufficit.Blazor.Client.Pages.Telephony.Monitor
{
    public partial class MonitorTelephonyBasePageComponent : BasePageComponent
    {
        protected override void OnAfterRender(bool firstRender)
        {
            // continuando o processo na base do componente
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                /*
                BreadcrumbService
                .Set<HomeBreadcrumb>()
                .Set<PagesBreadcrumb>()
                .Set<PageBreadcrumb>(new Dictionary<string, object>
                {
                    ["title"] = "Telefonia",
                    ["description"] = "Painel de telefonia",
                    ["link"] = "/pages/telephony"
                })
                .Set<PageBreadcrumb>(new Dictionary<string, object>
                {
                    ["title"] = "Monitor",
                    ["description"] = "Monitor de eventos",
                    ["link"] = "/pages/telephony/monitor"
                })
                .Set<PageBreadcrumb>(new Dictionary<string, object>
                {
                    ["title"] = Title,
                    ["description"] = Description,
                    ["link"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
                });

                BreadcrumbService.Description = Description;
                */
            }
        }
    }
}
