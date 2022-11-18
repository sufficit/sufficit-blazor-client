using MudBlazor;
using Sufficit.Blazor.BreadCrumb;

namespace Sufficit.Blazor.Client.Models.BreadCrumbs
{
    public class HomeBreadCrumb : BreadCrumbItem
    {
        public HomeBreadCrumb() {
            IsClearing = true;
            Title= "Entrada";
            Link = "/";
            Icon = Icons.Material.Filled.Home;
        }
    }
}
