using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using Sufficit.Blazor.BreadCrumb;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sufficit.Blazor.Client.Models.BreadCrumbs
{
    public static class BreadCrumbExtensions
    {
        public static BreadcrumbItem ToMudBlazorItem(this Sufficit.Blazor.BreadCrumb.BreadCrumbItem source)
        {
            return new BreadcrumbItem(source.Title, source.Link, source.Disabled, source.Icon);
        }

        public static IEnumerable<BreadcrumbItem> GetMudBlazorItems(this IBreadCrumbService source)
        {
            foreach(var item in source.Items) 
                yield return item.ToMudBlazorItem();
        }

        public static List<BreadcrumbItem> GetList(this IBreadCrumbService source)
            => GetMudBlazorItems(source).ToList();
    }
}
