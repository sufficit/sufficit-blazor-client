using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class RenderElement : ComponentBase
    {
        [Parameter]
        public object? Value { get; set; }
                
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private RenderFragment AddContent(string value) => builder =>
        {
            builder.AddContent(1, value);
        };

        public RenderFragment? GetRender()
        {
            if (Value is string valueString) return AddContent(valueString);
            else return ChildContent;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Value != null)
            {
                if (Value is Task<string> task)
                    Value = await task;
                else if (!(Value is string))
                    throw new Exception("value not supported");

                StateHasChanged();
            }
        }
    }
}
