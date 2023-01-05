using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.BreadCrumb;
using Sufficit.Blazor.Client.Models;
using Sufficit.Blazor.Client.Models.BreadCrumbs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages
{
    public class BasePageComponent : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
   
        [Inject]
        protected IBreadCrumbService BreadCrumbService { get; set; } = default!;

        protected virtual string Title { get; } = default!;

        protected virtual string Description { get; } = default!;

        protected virtual string Keywords { get; } = default!;

        protected override void OnInitialized()
        {
            OnBreadCrumbLoad();
        }

        protected virtual void OnBreadCrumbLoad()
        {
            BreadCrumbService
                .Set(new HomeBreadCrumb())
                .Set(new PagesBreadCrumb())
                .Set(new BreadCrumbItem()
                {
                    Title = Title,
                    Description = Description,
                    Disabled = true
                });
        }
    }
}
