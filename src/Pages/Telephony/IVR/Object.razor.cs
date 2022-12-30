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

        [CascadingParameter]
        public MudTheme? Theme { get; set; }

        protected Sufficit.Telephony.IVR? Item { get; set; }

        protected ICollection<Sufficit.Telephony.IVROption>? IVROptions { get; set; }

        protected bool IsLoading { get; set; } = false;

        private async void ContextViewChanged(Guid obj)
        {
            if (ContextView.ContextId != Guid.Empty)
            {
                Item = new Sufficit.Telephony.IVR();
                Item.Id = Guid.NewGuid();
                Item.IdContext = ContextView.ContextId;
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
                    Item = new Sufficit.Telephony.IVR();
                    Item.Id = Guid.NewGuid();
                    Item.IdContext = ContextView.ContextId;
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
            if (IVROptions == null)
                IVROptions = new List<IVROption>();

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
                this.IsLoading = true;
                var parameters = new DialogParameters();            

                try
                {
                    await APIClient.Telephony.IVR.Update(Item);
                
                    // await APIClient.Telephony.IVR.Update(ObjectId, IVROptions).Toast(Toasts, new UI.Material.Toasts.UpdateSuccessToast());
                    await InvokeAsync(StateHasChanged);

                    parameters.Add("content", "Está salvo com sucesso.");
                    DialogService.Show<StatusDialog>("Sucesso !", parameters);
                    this.IsLoading = false;
                } catch 
                {
                    parameters.Add("content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Fracassado !", parameters);
                    this.IsLoading = false;
                }
            }
        }        
           
        void IDisposable.Dispose()
        {
            ContextView.OnChanged -= ContextViewChanged;
        }
    }
}
