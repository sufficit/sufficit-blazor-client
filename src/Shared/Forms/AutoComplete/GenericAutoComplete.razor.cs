using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms.AutoComplete
{
    public abstract class GenericAutoComplete<T> : ComponentBase where T : IIdTitlePair
    {
        [Inject]
        protected APIClientService APIClient { get; set; } = default!;
              
        [Parameter]
        public Guid? Id { get; set; }

        [Parameter]
        public bool? Disabled { get; set; }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public virtual EventCallback<T?> ValueChanged { get; set; }

        protected virtual bool IsDisabled =>
            Disabled ?? false;

        protected virtual T? Value
        {
            get { return Generic is T current ? current : default; }
            set { Generic = value; }
        }


        protected IIdTitlePair? Generic { get; set; }

        protected override void OnParametersSet()
        {
            if (Id != null && Id != Guid.Empty)
            {
                UpdateInformation(Id.Value);
            }
        }

        protected async virtual Task<IEnumerable<T>> Search(string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var results = await APIClient.Contacts.Search(filter, 5, default);
                return results.OfType<T>();
            }

            return Array.Empty<T>();
        }

        protected async Task CurrentValueChanged(IIdTitlePair? current)
        {
            if (Generic != current)
            {
                if (current == null || current.Id == Guid.Empty)
                    Generic = default;
                else
                    Generic = current;

                if(ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
            }
        }

        protected async virtual void UpdateInformation(Guid initialValue)
        {
            if (initialValue != Guid.Empty)
            {
                Generic = await APIClient.Contacts.GetContact(initialValue);
                if(Generic != null)
                    await InvokeAsync(StateHasChanged);
            }
        }
    }
}
