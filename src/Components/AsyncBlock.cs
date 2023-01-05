using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    /// <summary>
    /// Execute an async task an render the result value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public partial class AsyncBlock<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TValue> : ComponentBase
    {
        [Parameter]
        [EditorRequired]
        public Task<TValue> Action { get; set; } = default!;
                
        [Parameter]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RenderFragment<TValue?>? ChildContent { get; set; }

        public TValue? Result { get; internal set; }

        /// <summary>
        /// Execute setted Action and updates Result
        /// </summary>
        /// <returns></returns>
        public async Task<TValue> Execute() => Result = await Action;

        protected override Task OnParametersSetAsync()
            => Execute();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
                builder.AddContent(0, ChildContent(Result));
        }
    }
}
