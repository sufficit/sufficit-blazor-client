using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Blazor.Components;
using System.Threading;

namespace Sufficit.Blazor.Client.Pages.Contacts
{
    public partial class Object : BasePageComponent
    {
        public const string RouteParameter = "pages/contacts/contact";

        protected override string Title => "Contato";

        protected override string Description => "Atributos do contato";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = "contactid")]
        public Guid ContactId { get; set; } = default!;

        protected IEnumerable<Sufficit.Contacts.Attribute>? Attributes { get; set; }

        #region TITLE AND DOCUMENT

        protected MudTextField<string> TitleReference { get; set; } = default!;

        protected MudTextField<string> DocumentReference { get; set; } = default!;

        protected string? ContactTitle { get; set; }
        
        protected bool HeaderChanged { get; set; }

        protected void OnTitleChanged(string? value)
        {
            if (ContactTitle != value)
            {
                ContactTitle = value;
                var title = Attributes?.FirstOrDefault(s => s.Key == Sufficit.Contacts.Attributes.Title)?.Value;
                HeaderChanged = ContactTitle != title;
            }
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (ContactId != Guid.Empty)
            {
                Attributes = await APIClient.Contacts.GetAttributes(ContactId, CancellationToken.None);
                if (!Attributes.Any())
                    throw new Exception($"Item not found: {ContactId}");

                ContactTitle = Attributes?.FirstOrDefault(s => s.Key == Sufficit.Contacts.Attributes.Title)?.Value ?? string.Empty;

                // Updating view
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task Save(MouseEventArgs _)
        {
            if (Attributes != null)
            {
                var parameters = new DialogParameters();  
                try
                {
                    //

                    parameters.Add("Content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", parameters);
                } 
                catch (Exception ex)
                {
                    parameters.Add("Ex", ex);
                    parameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", parameters);
                }
            }
        }        
    }
}
