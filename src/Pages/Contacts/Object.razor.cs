using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Blazor.Components;
using System.Threading;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using Sufficit.Blazor.Client.Components.Contacts;

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

        protected HashSet<Sufficit.Contacts.AttributeMonitor>? Attributes { get; set; }


        #region TITLE AND DOCUMENT

        protected bool CanUpdate { get; set; } = false;

        protected MudTextField<string> TitleReference { get; set; } = default!;

        protected MudTextField<string> DocumentReference { get; set; } = default!;

        protected AttributeMonitor ContactTitle { get; set; } = new AttributeMonitor();

        protected AttributeMonitor ContactDocument { get; set; } = new AttributeMonitor();
      
        protected void OnContactDocumentChanged(string? value)
        {
            ContactDocument.Value = new string(value?.Where(s => char.IsDigit(s)).ToArray());
        }

        protected bool HeaderChanged =>
            ContactTitle.HasChanged || ContactDocument.HasChanged;

        #endregion

        protected ContactAvatar AvatarReference { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (ContactId != Guid.Empty)
            {
                var attributes = await APIClient.Contacts.GetAttributes(ContactId, CancellationToken.None);
                if (!attributes.Any())
                    throw new Exception($"Item not found: {ContactId}");

                Attributes = attributes.ToMonitor().ToHashSet();

                if (!Attributes.Any(s => s.Key == Sufficit.Contacts.Attributes.Title))
                    Attributes.Add(ContactTitle);
                else
                    ContactTitle = Attributes.First(s => s.Key == Sufficit.Contacts.Attributes.Title);

                if (!Attributes.Any(s => s.Key == Sufficit.Contacts.Attributes.Document))
                    Attributes.Add(ContactDocument);
                else
                    ContactDocument = Attributes.First(s => s.Key == Sufficit.Contacts.Attributes.Document);

                CanUpdate = await APIClient.Contacts.CanUpdate(ContactId, CancellationToken.None);

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
                    var contact = new ContactWithAttributes(ContactId)
                    {
                        Attributes = Attributes.Where(s => s.HasChanged).ToHashSet<Sufficit.Contacts.Attribute>()
                    };

                    var id = await APIClient.Contacts.Update(contact, CancellationToken.None);
                    if (id != null && ContactId != id) { UpdateId(id.Value); }

                    parameters.Add("Content", $"({contact.Attributes.Count}) Atributos atualizados com sucesso.");
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

        protected void UpdateId(Guid id)
        {            
            var destination = NavigationManager.GetUriWithQueryParameter(nameof(ContactId).ToLower(), id);
            NavigationManager.NavigateTo(destination, false);
        }

        protected async void OnInputFileChanged(InputFileChangeEventArgs args)
        {
            using (var ms = new MemoryStream())
            {
                await args.File.OpenReadStream().CopyToAsync(ms);
                ms.Position = 0;
                await APIClient.Contacts.Update(ContactId, ms, CancellationToken.None);                
                await InvokeAsync(AvatarReference.StateHasChanged);
            }
        }            
    }
}
