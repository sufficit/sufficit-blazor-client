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
using System.Globalization;
using System.Security.Cryptography;
using Sufficit.Telephony.DIDs;
using Sufficit.Blazor.Components;

namespace Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing
{
    [Authorize(Roles = "telephony")]
    public partial class Object : TelephonyBasePageComponent, IDisposable, IPage
    {
        public const string RouteParameter = "/pages/telephony/did";

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

        protected CultureInfo Culture = new CultureInfo("pt-BR");

        protected Converter<DateTime> ConverterDateTime = new Converter<DateTime>
        {
            SetFunc = value => value.ToString("yyyy-MM-dd"),
            GetFunc = DateTime.Parse
        };

        protected Sufficit.Telephony.DirectInwardDialing? Item { get; set; }

        protected IMask MaskOnlyNumbers = new RegexMask(@"^\d+$");

        private async void ContextViewChanged(Guid obj)
        {
            if (ContextView.ContextId != Guid.Empty)
            {
                Item = new Sufficit.Telephony.DirectInwardDialing
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

        #region PROPERTIES CHANGED

        protected void OnProviderChange(Guid newValue)
        {
            Item!.ProviderId = newValue;
            PropertiesPending = true;
        }

        protected void OnMaxChannelsChange(int newValue)
        {
            Item!.MaxChannels = newValue;
            PropertiesPending = true;
        }

        protected void OnRegisterChange(DateTime newValue)
        {
            Item!.Register = newValue;
            PropertiesPending = true;
        }

        protected void OnBilledChange(bool newValue)
        {
            Item!.Billed = newValue;
            PropertiesPending = true;
        }

        protected void OnDescriptionChange(string? newValue)
        {
            Item!.Description = newValue;
            ExtraPending = true;
        }

        protected void OnTagsChange(string? newValue)
        {
            Item!.Tags = newValue;
            ExtraPending = true;
        }

        protected void OnFilterChange(string? newValue)
        {
            Item!.Filter = newValue;
            FilterPending = true;
        }

        #endregion

        protected override void OnParametersSet()
        {            
            // should create a new item
            if (Item == null)
            {
                if (ObjectId == Guid.Empty && ContextView.ContextId != Guid.Empty)
                {
                    Item = new Sufficit.Telephony.DirectInwardDialing
                    {
                        ContextId = ContextView.ContextId                        
                    };
                }
            }

            Providers ??= new List<IContact>()
                {
                    new Contact() { Id = Guid.Parse("7b0f2ebd-de91-4688-b206-a97acab03d11"), Title = "Flux Telecom"},
                    new Contact() { Id = Guid.Parse("73d4e817-90bf-4f17-82f3-87f52115c969"), Title = "BRDID - TIP | TVN"},
                    new Contact() { Id = Guid.Parse("01fe04e5-ad3e-43b9-b2e5-6556fd982e62"), Title = "TIP Brasil Telecom"},
                    new Contact() { Id = Guid.Parse("68d7f117-72f7-4484-b4d8-e6cc438723b6"), Title = "America Net / DATORA"},
                    new Contact() { Id = Guid.Parse("a80507c4-4732-493c-b084-12ec5ba37981"), Title = "Citta Telecom"},
                    new Contact() { Id = Guid.Parse("bbdb62fe-24bf-4974-9344-be386f9369ba"), Title = "VoipLinkTronic (ALGAR)"},
                    new Contact() { Id = Guid.Parse("2604251f-a309-4dfd-b1d2-7610cd57258f"), Title = "Transit Telecom"},
                    new Contact() { Id = Guid.Parse("f9fc7cbb-7290-4fba-95cb-ff06bf2aafc6"), Title = "GT Group International Brasil Telecomunicações LTDA (GTGI)"},
                    new Contact() { Id = Guid.Parse("7d6de848-e576-4564-812f-b36ac1b2ed9f"), Title = "GT Group Faturamento Individual"},
                    new Contact() { Id = Guid.Parse("4d716201-ce36-4a8c-8123-5c620abba23f"), Title = "DirectCall"},
                    new Contact() { Id = Guid.Parse("2dcd015b-9538-469c-9434-138ba176efa9"), Title = "Voip do Brasil"},
                    new Contact() { Id = Guid.Parse("ba7dbdeb-a1aa-4a21-b3cc-caacd818f3ec"), Title = "Skype DID"},
                    new Contact() { Id = Guid.Parse("ffb71292-8d68-47b6-bf54-2b57a72581bf"), Title = "Fale Sempre Mais"},
                    new Contact() { Id = Guid.Parse("d21cfb04-9d37-473b-837c-67591a26feed"), Title = "Sufficit Soluções"},
                    new Contact() { Id = Guid.Parse("934f107b-4008-46e2-b011-81a021e257e0"), Title = "Red Telecom"}
                };
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
                        await ContextView.Update(Item.ContextId);
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

        protected void OnDestinationChanged(IDestination? value)
        {
            if (Item != null)
            {
                if (value != null)
                {
                    if (Item.Asterisk != value.Asterisk)
                    {
                        Item.Asterisk = value.Asterisk;
                        Item.dstid = value.Id?.ToString() ?? string.Empty;
                        Item.dstclasse = value.TypeName;
                        DestinationPending = true;
                    }
                }
                else
                {
                    Item.Asterisk = string.Empty;
                }
            }
        }

        protected async Task UpdateProperties(MouseEventArgs _)
        {
            if (Item != null)
            {
                var parameters = new DialogParameters();  
                try
                {
                    // DirectInwardDialingProperties
                    // updating basic info
                    await APIClient.Telephony.DID.Properties(Item.Id, Item, default);
                    PropertiesPending = false;

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

        protected async Task UpdateOwner(MouseEventArgs _)
        {
            if (Item != null)
            {
                var requestParameters = new OwnerUpdateParameters();
                requestParameters.Id = Item.Id;
                requestParameters.OwnerId = Item.OwnerId;

                var dialogParameters = new DialogParameters();
                try
                {
                    // updating basic info
                    await APIClient.Telephony.DID.Owner(requestParameters, default);
                    OwnerPending = false;

                    dialogParameters.Add("Content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", dialogParameters);
                }
                catch (Exception ex)
                {
                    dialogParameters.Add("Ex", ex);
                    dialogParameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                }
            }
        }

        protected async Task UpdateExtra(MouseEventArgs _)
        {
            if (Item != null)
            {
                var requestParameters = new ExtraUpdateParameters();
                requestParameters.Id = Item.Id;
                requestParameters.Description = Item.Description;
                requestParameters.Tags = Item.Tags;

                var dialogParameters = new DialogParameters();
                try
                {
                    // updating basic info
                    await APIClient.Telephony.DID.Extra(requestParameters, default);
                    ExtraPending = false;

                    dialogParameters.Add("Content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", dialogParameters);
                }
                catch (Exception ex)
                {
                    dialogParameters.Add("Ex", ex);
                    dialogParameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                }
            }
        }

        protected async Task UpdateFilter(MouseEventArgs _)
        {
            if (Item != null)
            {
                var requestParameters = new FilterUpdateParameters();
                requestParameters.Id = Item.Id;
                requestParameters.Filter = Item.Filter;

                var dialogParameters = new DialogParameters();
                try
                {
                    // updating basic info
                    await APIClient.Telephony.DID.Filter(requestParameters, default);
                    ExtraPending = false;

                    dialogParameters.Add("Content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", dialogParameters);
                }
                catch (Exception ex)
                {
                    dialogParameters.Add("Ex", ex);
                    dialogParameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                }
            }
        }

        protected async Task UpdateDestination(MouseEventArgs _)
        {
            if (Item != null)
            {
                var destination = new DestinationBase();
                destination.Asterisk = Item.Asterisk;
                destination.TypeName = Item.dstclasse;
                if(Guid.TryParse(Item.dstid, out Guid dstId))
                    destination.Id = dstId;

                var dialogParameters = new DialogParameters();
                try
                {
                    // updating basic info
                    await APIClient.Telephony.DID.Destination(Item.Id, destination, default);
                    DestinationPending = false;

                    dialogParameters.Add("Content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", dialogParameters);
                }
                catch (Exception ex)
                {
                    dialogParameters.Add("Ex", ex);
                    dialogParameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                }
            }
        }

        protected void GoToHistory(MouseEventArgs _)
        {
            if (Item != null)
            {
                var query = $"classname={nameof(DirectInwardDialing)}&event.contextid={Item.Id}";
                NavigationManager.NavigateTo<Logging.Events>(false, query);
            }
        }

        /// <summary>
        /// Indicates that owner has changed and not saved yet
        /// </summary>
        protected bool OwnerPending { get; set; } 

        protected bool ExtraPending { get; set; }

        protected bool FilterPending { get; set; }

        protected bool DestinationPending { get; set; }

        protected bool PropertiesPending { get; set; }

        protected void OwnerChanged(IContact? contact)
        {
            if (Item != null)
            {
                var newOwnerId = contact?.Id;
                if (Item.OwnerId != newOwnerId)
                {
                    Item.OwnerId = newOwnerId;
                    OwnerPending = true;
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
