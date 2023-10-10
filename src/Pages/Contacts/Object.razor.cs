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
    public partial class Object : BasePageComponent, IDisposable
    {
        public const string RouteParameter = "pages/contacts/contact";

        protected override string Title => "IVR";

        protected override string Description => "Opções do painel de eventos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = "contactid")]
        public Guid ContactId { get; set; } = default!;

        protected Sufficit.Contacts.Contact? Item { get; set; }

        protected ICollection<Sufficit.Telephony.IVROption>? IVROptions { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (ContactId != Guid.Empty)
            {
                Item = await APIClient.Contacts.GetContact(ContactId, CancellationToken.None);
                if (Item == null)
                    throw new Exception($"Item not found: {ContactId}");

                // Updating view
                await InvokeAsync(StateHasChanged);
            }                      
        }

        protected void NewOption(MouseEventArgs _)
        {
            IVROptions ??= new List<IVROption>();
            IVROptions.Add(new IVROption());
            StateHasChanged();
        }

        protected void DelOption(IVROption option)
        {
            if(IVROptions?.Remove(option) ?? false)
                StateHasChanged();
        }

        protected async Task Save(MouseEventArgs _)
        {
            if (Item != null)
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
           
        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
