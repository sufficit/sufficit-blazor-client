using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Models
{
    [RootBreadcrumb]
    public class HomeBreadcrumb : RootBreadcrumb
    {
        public override void Configure(BreadcrumbBuilder builder)
        {
            builder.LeftIcon("fas fa-home");
            builder.LeftAttributes = new Dictionary<string, object>
            {
                ["style"] = "margin-right: 0.5em;"
            };
            builder.Link("Entrada", string.Empty, new KeyValuePair<string, object>("class", "opacity-5 text-dark"));
        }
    }
    public class PagesBreadcrumb : Breadcrumb
    {
        public override void Configure(BreadcrumbBuilder builder)
        {
            builder.Text("Páginas", new KeyValuePair<string, object>("class", "opacity-5 text-dark"));
        }
    }

    public class PageBreadcrumb : Breadcrumb
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public string Link { get; set; }

        public override void Configure(BreadcrumbBuilder builder)
        {
            builder.Link(Title, Link, 
                new KeyValuePair<string, object>("class", "opacity-5 text-dark"),
                new KeyValuePair<string, object>("data-bs-toggle", "tooltip"),
                new KeyValuePair<string, object>("data-bs-placement", "bottom"),
                new KeyValuePair<string, object>("title", Description)
                );
        }
    }
}
