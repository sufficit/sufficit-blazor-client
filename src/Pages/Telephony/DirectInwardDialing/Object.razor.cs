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

        public static string GetLink(Guid? objectid)
        {
            var link = RouteParameter;
            if (objectid.HasValue)
                link += $"?id={objectid}";
            return link.TrimEnd('&');
        }

        protected IEnumerable<IIdTitlePair>? Providers { get; set; }

        protected CultureInfo Culture = new CultureInfo("pt-BR");

        protected Converter<DateTime> ConverterDateTime = new Converter<DateTime>
        {
            SetFunc = value => value.ToString("yyyy-MM-dd"),
            GetFunc = DateTime.Parse!
        };

        protected Sufficit.Telephony.DirectInwardDialing? Item { get; set; }

        protected IMask MaskOnlyNumbers = new RegexMask(@"^\d+$");

        private async void ContextViewChanged(Guid? value)
        {
            var contextid = value.GetValueOrDefault();
            if (contextid != Guid.Empty)
            {
                Item = new Sufficit.Telephony.DirectInwardDialing
                {
                    Id = Guid.NewGuid(),
                    ContextId = contextid
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
            base.OnParametersSet();

            // should create a new item
            if (Item == null)
            {
                var contextid = ContextView.ContextId.GetValueOrDefault();
                if (ObjectId == Guid.Empty && contextid != Guid.Empty)
                {
                    Item = new Sufficit.Telephony.DirectInwardDialing
                    {
                        ContextId = contextid
                    };
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (Providers == null)
                Providers = await APIClient.Telephony.Carriers();
            
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
      

            // tracking context changes
            ContextView.OnChanged -= ContextViewChanged;
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
