using Sufficit.Blazor.BreadCrumb;
using Sufficit.Blazor.Client.Models;
using Sufficit.Blazor.Client.Models.BreadCrumbs;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    public partial class TelephonyBasePageComponent : BasePageComponent
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
