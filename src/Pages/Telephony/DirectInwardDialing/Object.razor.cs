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
using Sufficit.Contacts;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class Object : TelephonyBasePageComponent, IDisposable
    {
        protected override string Title => "DID | Entrada";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        protected IContextView ContextView { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = "id")]
        public Guid ObjectId { get; set; } = default!;

        protected IEnumerable<IContact>? Providers { get; set; }

        protected Sufficit.Telephony.DirectInwardDialingV1? Item { get; set; }

        protected IMask MaskOnlyNumbers = new RegexMask(@"^\d+$");

        private async void ContextViewChanged(Guid obj)
        {
            if (ContextView.ContextId != Guid.Empty)
            {
                Item = new Sufficit.Telephony.DirectInwardDialingV1
                {
                    Id = Guid.NewGuid(),
                    ContextId = ContextView.ContextId
                };
            } 
            else 
            {
                Item = null;
            }            

            await InvokeAsync(StateHasChanged);
        }

        protected void OnProviderChange(Guid selected)
        {
            Item!.ProviderId = selected;
        }

        protected override void OnParametersSet()
        {            
            // should create a new item
            if (Item == null)
            {
                if (ObjectId == Guid.Empty && ContextView.ContextId != Guid.Empty)
                {
                    Item = new Sufficit.Telephony.DirectInwardDialingV1
                    {
                        ContextId = ContextView.ContextId                        
                    };
                }
            }

            if(Providers == null)
            {
                Providers = new List<IContact>()
                {
                    new Contact() { ID= Guid.Parse("7b0f2ebd-de91-4688-b206-a97acab03d11"), Title = "Flux Telecom"},
                    new Contact() { ID= Guid.Parse("73d4e817-90bf-4f17-82f3-87f52115c969"), Title = "BRDID - TIP | TVN"},
                    new Contact() { ID= Guid.Parse("01fe04e5-ad3e-43b9-b2e5-6556fd982e62"), Title = "TIP Brasil Telecom"},
                    new Contact() { ID= Guid.Parse("68d7f117-72f7-4484-b4d8-e6cc438723b6"), Title = "America Net / DATORA"},
                    new Contact() { ID= Guid.Parse("a80507c4-4732-493c-b084-12ec5ba37981"), Title = "Citta Telecom"},
                    new Contact() { ID= Guid.Parse("bbdb62fe-24bf-4974-9344-be386f9369ba"), Title = "VoipLinkTronic (ALGAR)"},
                    new Contact() { ID= Guid.Parse("2604251f-a309-4dfd-b1d2-7610cd57258f"), Title = "Transit Telecom"},
                    new Contact() { ID= Guid.Parse("f9fc7cbb-7290-4fba-95cb-ff06bf2aafc6"), Title = "GT Group International Brasil Telecomunicações LTDA (GTGI)"},
                    new Contact() { ID= Guid.Parse("7d6de848-e576-4564-812f-b36ac1b2ed9f"), Title = "GT Group Faturamento Individual"},
                    new Contact() { ID= Guid.Parse("4d716201-ce36-4a8c-8123-5c620abba23f"), Title = "DirectCall"},
                    new Contact() { ID= Guid.Parse("2dcd015b-9538-469c-9434-138ba176efa9"), Title = "Voip do Brasil"},
                    new Contact() { ID= Guid.Parse("ba7dbdeb-a1aa-4a21-b3cc-caacd818f3ec"), Title = "Skype DID"},
                    new Contact() { ID= Guid.Parse("ffb71292-8d68-47b6-bf54-2b57a72581bf"), Title = "Fale Sempre Mais"},
                    new Contact() { ID= Guid.Parse("d21cfb04-9d37-473b-837c-67591a26feed"), Title = "Sufficit Soluções"},
                    new Contact() { ID= Guid.Parse("934f107b-4008-46e2-b011-81a021e257e0"), Title = "Red Telecom"}
                };
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (ObjectId != Guid.Empty)
            {
                Item = await APIClient.Telephony.DID.ById(ObjectId);
                if (Item != null)
                {
                    // updating context view from ivr context
                    if (Item.ContextId != ContextView.ContextId)
                    {
                        // changing before render, to avoid 
                        ContextView.Update(Item.ContextId);
                    }
                } else throw new Exception($"Item not found: { ObjectId }");

                // Updating view
                await InvokeAsync(StateHasChanged);
            }

            if(Providers == null)
            {
                // Should create an endpoint for that !
                //Providers = await APIClient.Telephony.DID.
            }

            // tracking context changes
            ContextView.OnChanged += ContextViewChanged;                       
        }

        protected async Task Save(MouseEventArgs _)
        {
            if (Item != null)
            {
                var parameters = new DialogParameters();  
                try
                {
                    // updating basic info
                    // await APIClient.Telephony.DID.Update(Item);     
                    
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
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
