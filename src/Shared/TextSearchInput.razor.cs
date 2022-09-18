using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Blazor.UI.Material;
using Sufficit.Blazor.UI.Material.Components;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class TextSearchInput : ComponentBase, IDisposable
    {
        private bool _disabled;

        [Parameter]
        public string? Value { get; set; }

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        protected InputGroup? InputGroupReference { get; set; } = default!;

        protected void SearchInputChanged(string? value)
        {
            if (TextSearch != null)
                TextSearch.Update(value);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender) return;

            if (TextSearch != null)
            {
                TextSearch.OnChanged += TextSearchChanged;
                TextSearch.OnValueChanged += TextSearchValueChanged;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (TextSearch != null)
            {
                _disabled = !TextSearch.CanSearch;
                Value = TextSearch.Value;
            }
        }

        protected async void TextSearchChanged(bool value)
        {
            if (_disabled != value)
            {
                _disabled = !value;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async void TextSearchValueChanged(string? value)
        {
            if (Value != value)
            {
                Value = value;
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            if (TextSearch != null)
            {
                TextSearch.OnChanged -= TextSearchChanged;
                TextSearch.OnValueChanged -= TextSearchValueChanged;
            }
        }

    }
}
