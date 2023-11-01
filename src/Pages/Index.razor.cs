using Sufficit.Blazor.BreadCrumb;
using Sufficit.Blazor.Components;
using System;

namespace Sufficit.Blazor.Client.Pages
{
    public partial class Index : BasePageComponent, IPage
    {
        public const string RouteParameter = "/";

        protected string Presentation { get; set; } = default!;

        protected string Style
            => $"min-height: 300px; background-position: center; background-size: cover; background-image: url('{Presentation}');";

        protected override void OnBreadCrumbLoad()
        {
            BreadCrumbService
                    .Set(new HomeBreadCrumb());
        }

        protected override void OnInitialized()
        {            
            // initialize base
            base.OnInitialized();

            var index = new Random().Next(1, 3);
            Presentation = $"/assets/img/presentations/{index}.jpg";            
        }
    }
}
