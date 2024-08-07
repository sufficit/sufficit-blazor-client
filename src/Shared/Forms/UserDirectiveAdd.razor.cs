﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms
{
    public partial class UserDirectiveAdd : ComponentBase
    {
        [Inject]
        public APIClientService APIClient { get; set; } = default!;

        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

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
            return await APIClient.Identity.GetDirectives(value, 10, cancellationToken);
        }

        private async Task<IEnumerable<IContact>> GetContacts(string value, CancellationToken cancellationToken)
        {
            var items = new HashSet<IContact>();

            if ("todos".Contains(value))            
                items.Add(new Contact() { Title = "* Todos" });            

            // if text is null or empty, show complete list
            foreach (var contact in await APIClient.Contacts.Search(value, 10, cancellationToken))            
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
                        var policies = await BIService.GetUserPolicies(User, default);
                        var exists = policies.Any(s => s.IDDirective == Directive.ID && s.IDContext == Contact.Id);
                        if (exists)
                        {
                            Snackbar.Add("Já possui", Severity.Error);
                            return;
                        }

                        await BIService.UpdateUserPolicy(User, Directive.Key, Contact.Id, default);

                        // Refresh
                        await OnChanged.InvokeAsync();
                    }
                    else
                    {
                        Snackbar.Add("Contexto em branco.", Severity.Error);
                    }
                }
                else
                {
                    Snackbar.Add("Diretiva em branco.", Severity.Error);
                }
            } 
            else
            {
                Snackbar.Add("Usuário não selecionado", Severity.Error);
            }
        }
    }
}
