using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Contacts
{
    public partial class SuffContactTitle : ComponentBase
    {
        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [EditorRequired]
        [Parameter]
        public Guid ReferenceId { get; set; }

        protected async Task<string> GetContactTitle(Guid idcontact, CancellationToken cancellationToken = default)
        {
            if (idcontact == Guid.Empty) return "* Todos";

            Sufficit.Contacts.IContact? contact = null;
            try
            {
                contact = await BIService.GetContact(idcontact, cancellationToken);
                if (contact == null) return string.Empty;
            } catch (OperationCanceledException) { }
            
            return contact?.Title ?? "* Desconhecido";
        }
    }
}
