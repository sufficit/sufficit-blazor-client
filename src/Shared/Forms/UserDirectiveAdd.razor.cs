using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        protected bool Saving = false;

        private async Task<IEnumerable<IDirective>> GetDirectives(string value, CancellationToken cancellationToken)
        {
            // if text is null or empty, show complete list
            return await Endpoints.Identity.GetDirectives(value, 10, cancellationToken);
        }

        private async Task<IEnumerable<IContact>> GetContacts(string value, CancellationToken cancellationToken)
        {
            // if text is null or empty, show complete list
            return await Endpoints.Contact.Search(value, 10, cancellationToken);
        }


        protected async Task OnAddClick(MouseEventArgs e)
        {/*
            var user = UserSelected;
            var directive = InputDirective?.Selected;
            var context = InputContext?.Selected;

            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(directive))
                {
                    if (!string.IsNullOrWhiteSpace(context))
                    {
                        if (Guid.TryParse(context, out Guid idcontext))
                        {
                            await BIService.UpdateUserPolicy(user, directive, idcontext, default);
                            await SelectUser(UserSelected, default);
                        }
                        else
                        {
                            throw new FormValidationException("Contexto inválido.");
                        }
                    }
                    else
                    {
                        throw new FormValidationException("Contexto em branco.");
                    }
                }
                else
                {
                    throw new FormValidationException("Diretiva em branco.");
                }
            } 
            else
            {
                throw new FormValidationException("Usuário não selecionado");
            }
            */
        }

    }
}
