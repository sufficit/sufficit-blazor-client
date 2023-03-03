using Sufficit.Blazor.BreadCrumb;
using Sufficit.Blazor.Components;
using Sufficit.Blazor.Client.Models;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    public abstract class TelephonyBasePageComponent : BasePageComponent
    {
        protected override void OnBreadCrumbLoad()
        {
            BreadCrumbService
                .Set<HomeBreadCrumb>()
                .Set<PagesBreadCrumb>()
                .Set(new BreadCrumbItem()
                {
                    Title = "Telefonia",
                    Description = "Painel de telefonia",
                    Link = "/pages/telephony"
                })
                .Set(new BreadCrumbItem()
                {
                    Title = Title,
                    Description = Description,
                    Disabled = true
                });
        }
    }
}
