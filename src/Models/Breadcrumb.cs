using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Models
{
    public abstract class Breadcrumb : ComponentBase
    {
        private readonly BreadcrumbBuilder _breadcrumbBuilder = new();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (_breadcrumbBuilder.Url == null)
            {
                RenderFragmentText(builder);
            }
            else
            {
                RenderFragmentNavLink(builder);
            }
        }

        private void RenderFragmentNavLink(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "href", _breadcrumbBuilder.Url);
            builder.AddAttribute(2, nameof(NavLink.Match), NavLinkMatch.All);
            builder.AddMultipleAttributes(3, _breadcrumbBuilder.Attributes);

            RenderLeftIcon(builder);

            builder.AddContent(4, _breadcrumbBuilder.Title);

            RenderRightIcon(builder);

            builder.CloseElement();
        }

        private void RenderFragmentText(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");

            RenderLeftIcon(builder);

            builder.OpenElement(1, "span");
            builder.AddMultipleAttributes(2, _breadcrumbBuilder.Attributes?.ToList());

            builder.AddContent(3, _breadcrumbBuilder.Title);

            builder.CloseComponent();

            RenderRightIcon(builder);

            builder.CloseElement();
        }

        private void RenderLeftIcon(RenderTreeBuilder builder)
            => RenderIcon(builder,
                _breadcrumbBuilder.LeftIcon,
                _breadcrumbBuilder.LeftAction,
                _breadcrumbBuilder.LeftAttributes);

        private void RenderRightIcon(RenderTreeBuilder builder)
            => RenderIcon(builder,
                _breadcrumbBuilder.RightIcon,
                _breadcrumbBuilder.RightAction,
                _breadcrumbBuilder.RightAttributes);

        private void RenderIcon(RenderTreeBuilder builder, string icon, Action action, IEnumerable<KeyValuePair<string, object>> attributes)
        {
            if (icon != null)
            {
                builder.OpenRegion(0);
                builder.OpenElement(1, action != null ? "button" : "i");
                builder.AddAttribute(2, "class", $"{icon}");
                builder.AddMultipleAttributes(3, attributes);
                if (action != null)
                {
                    builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, action));
                }
                builder.CloseElement();
                builder.CloseRegion();
            }
        }

        public override Task SetParametersAsync(ParameterView parameters)
            => base.SetParametersAsync(parameters);

        protected override void OnParametersSet()
            => Configure(_breadcrumbBuilder);

        protected override Task OnParametersSetAsync()
            => ConfigureAsync(_breadcrumbBuilder);

        public virtual Task ConfigureAsync(BreadcrumbBuilder builder)
            => Task.CompletedTask;

        public virtual void Configure(BreadcrumbBuilder builder)
        {
        }
    }
}
