using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Blazor.Client.Pages.Telephony.DirectInwardDialing;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Telephony.DIDs;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms
{
    public partial class DIDContextUpdate : ComponentBase
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [EditorRequired]
        [Parameter]
        public Sufficit.Telephony.DirectInwardDialing? Item { get; set; } = default!;

        /// <summary>
        /// Should UnBind button be visible ?
        /// </summary>
        protected bool UnBindIsVisible 
            => Item != null && Item.ContextId != Guid.Empty;

        protected override void OnParametersSet()
        {
            if (Item != null)
                newContextId = Item.ContextId;             
        }

        protected bool UpdatePending
            => Item != null && Item.ContextId != newContextId;

        private Guid newContextId;

        protected void OnValueChanged(IIdTitlePair? pair)
        {
            if (Item == null) return;
            if (pair == null || pair.Id == Guid.Empty) return;
                        
            newContextId = pair.Id;            
        }

        protected async Task Update(MouseEventArgs _)
        {
            if (Item != null)
            {
                var dialogParameters = new DialogParameters();
                try
                {
                    var parameters = new ContextUpdateParameters();
                    parameters.Id = Item.Id;
                    parameters.ContextId = newContextId;
                    parameters.Clear = true;

                    // updating basic info
                    await APIClient.Telephony.DID.Context(parameters, default);
                    Item.Id = parameters.ContextId;

                    dialogParameters.Add("Content", "Está salvo com sucesso.");
                    var dreference = await DialogService.ShowAsync<StatusDialog>("Sucesso !", dialogParameters);
                    await dreference.Result;

                    Navigation.NavigateTo<DashBoard>();
                }
                catch (Exception ex)
                {
                    dialogParameters.Add("Ex", ex);
                    dialogParameters.Add("Content", "Falha ao salvar.");
                    DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                }

                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task UnBind(MouseEventArgs _)
        {
            if (Item != null)
            {
                var optionsIsolate = new MessageBoxOptions();
                optionsIsolate.Title = "Desvincular e Isolar";
                optionsIsolate.Message = 
                    @"Aproveitando que vamos desvincular essa DID, 
                    deseja deixa-la isolada por um período para evitar que seja revendida imediatamente ?";

                optionsIsolate.YesText = "sim";
                optionsIsolate.NoText = "não";
                optionsIsolate.CancelText = "cancelar";

                var isolate = await DialogService.ShowMessageBox(optionsIsolate);
                if (isolate != null)
                {
                    var dialogParameters = new DialogParameters();
                    try
                    {
                        var parameters = new ContextUpdateParameters();
                        parameters.Id = Item.Id;
                        parameters.ContextId = Guid.Empty;
                        parameters.Clear = true;

                        if (isolate.Value)                        
                            parameters.Expire = DateTime.UtcNow.AddMonths(3);

                        // updating basic info
                        await APIClient.Telephony.DID.Context(parameters, default);
                        Item.Id = parameters.ContextId;

                        dialogParameters.Add("Content", "Está salvo com sucesso.");
                        var dreference = await DialogService.ShowAsync<StatusDialog>("Sucesso !", dialogParameters);
                        await dreference.Result;
                        Navigation.NavigateTo<DashBoard>();
                    }
                    catch (Exception ex)
                    {
                        dialogParameters.Add("Ex", ex);
                        dialogParameters.Add("Content", "Falha ao salvar.");
                        DialogService.Show<StatusDialog>("Falha !", dialogParameters);
                    }

                    await InvokeAsync(StateHasChanged);
                }      
            }
        }
    }
}
