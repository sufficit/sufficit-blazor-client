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

namespace Sufficit.Blazor.Client.Pages.Provisioning
{
    public partial class Device : BasePageComponent
    {
        public const string RouteParameter = "/pages/provisioning/device";

        protected override string Title => "Dispositivo";

        protected override string Description => "Informações sobre um dispositivo a ser provisionado";

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [EditorRequired]
        [Parameter, SupplyParameterFromQuery(Name = "macaddress")]
        public string MACAddress { get; set; } = default!;

        protected Sufficit.Telephony.Device? Item { get; set; }

        protected Sufficit.Telephony.EndPoint? EndPoint { get; set; }

        protected Sufficit.Contacts.Contact? Context { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (Item == null)
            {
                Item = await APIClient.Provisioning.ByMAC(MACAddress, CancellationToken.None);
                if (Item == null)   
                    throw new Exception($"Item not found: {MACAddress}");

                if (Item.ExtensionId.HasValue)
                {
                    var parameters = new EndPointSearchParameters();
                    parameters.Id = Item.ExtensionId.Value;
                    parameters.Limit = 1;
                    var endpoints = await APIClient.Telephony.EndPoint.GetEndPoints(parameters);
                    if (endpoints.Any())
                        EndPoint = endpoints.FirstOrDefault();
                    else throw new Exception("device extension not found");
                }

                // Updating view
                await InvokeAsync(StateHasChanged);
            }                     
        }

        protected async Task Save(MouseEventArgs _)
        {
            if (Item != null)
            {
                var parameters = new DialogParameters();  
                try
                {
                    // updating basic info
                    //await APIClient.Telephony.IVR.Update(Item);     
                    
                    // updating options
                    //await APIClient.Telephony.IVR.Update(ObjectId, IVROptions);

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
