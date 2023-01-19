using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms
{
    public partial class UserDirectiveAdd : ComponentBase
    {
        [Inject]
        public APIClientService Endpoints { get; set; } = default!;

        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public Sufficit.Identity.Client.User User { get; set; } = default!;

        [Parameter]
        public EventCallback OnChanged { get; set; }

        protected Sufficit.Identity.IDirective? Directive;
        protected Sufficit.Contacts.IContact? Contact;

        protected bool Saving = false;

        private async Task<IEnumerable<IDirective>> GetDirectives(string value, CancellationToken cancellationToken)
        {
            // if text is null or empty, show complete list
            return await Endpoints.Identity.GetDirectives(value, 10, cancellationToken);
        }

        private async Task<IEnumerable<IContact>> GetContacts(string value, CancellationToken cancellationToken)
        {
            var items = new HashSet<IContact>();

            if ("todos".Contains(value))            
                items.Add(new Contact() { Title = "* Todos" });            

            // if text is null or empty, show complete list
            foreach(var contact in await Endpoints.Contact.Search(value, 10, cancellationToken))            
                items.Add(contact);

            return items;
        }


        protected async Task OnAddClick(MouseEventArgs e)
        { 
            if (User != null)
            {
                if (Directive != null)
                {
                    if (Contact != null)
                    {
                        await BIService.UpdateUserPolicy(User, Directive.Key, Contact.ID, default);

                        // Refresh
                        await OnChanged.InvokeAsync();
                    }
                    else
                    {
                        throw new Exception("Contexto em branco.");
                    }
                }
                else
                {
                    throw new Exception("Diretiva em branco.");
                }
            } 
            else
            {
                throw new Exception("Usuário não selecionado");
            }
        }
    }
}
