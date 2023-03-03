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

namespace Sufficit.Blazor.Client.Pages.Telephony.IVR
{
    [Authorize(Roles = "telephony")]
    public partial class Object : TelephonyBasePageComponent, IDisposable
    {
        public const string RouteParameter = "/pages/telephony/ivr";

        protected override string Title => "IVR";

        protected override string Description => "Opções do painel de eventos";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        protected IContextView ContextView { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = "id")]
        public Guid ObjectId { get; set; } = default!;

        protected Sufficit.Telephony.IVR? Item { get; set; }

        protected ICollection<Sufficit.Telephony.IVROption>? IVROptions { get; set; }

        private async void ContextViewChanged(Guid obj)
        {
            if (ContextView.ContextId != Guid.Empty)
            {
                Item = new Sufficit.Telephony.IVR
                {
                    Id = Guid.NewGuid(),
                    IdContext = ContextView.ContextId
                };
            } 
            else 
            {
                Item = null;
            }            

            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // should create a new item
            if (Item == null)
            {
                if (ObjectId == Guid.Empty && ContextView.ContextId != Guid.Empty)
                {
                    Item = new Sufficit.Telephony.IVR
                    {
                        Id = Guid.NewGuid(),
                        IdContext = ContextView.ContextId
                    };
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            if (ObjectId != Guid.Empty)
            {
                Item = await APIClient.Telephony.IVR.Find(ObjectId);
                if (Item != null)
                {
                    IVROptions = (await APIClient.Telephony.IVR.GetOptions(Item.Id)).ToList();

                    // updating context view from ivr context
                    if (Item.IdContext != ContextView.ContextId)
                    {
                        // changing before render, to avoid 
                        await ContextView.Update(Item.IdContext);
                    }
                } else throw new Exception($"Item not found: { ObjectId }");

                // Updating view
                await InvokeAsync(StateHasChanged);
            }

            // tracking context changes
            ContextView.OnChanged += ContextViewChanged;                       
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
                    // updating basic info
                    await APIClient.Telephony.IVR.Update(Item);     
                    
                    // updating options
                    await APIClient.Telephony.IVR.Update(ObjectId, IVROptions);

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
