﻿using Microsoft.AspNetCore.Authorization;
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
using Sufficit.Contacts;
using Sufficit.Provisioning;

namespace Sufficit.Blazor.Client.Pages.Provisioning
{
    public partial class Device : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/provisioning/device";

        public const string Title = "Dispositivo";
        protected override string Description => "Informações sobre um dispositivo a ser provisionado";

        #endregion

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [EditorRequired]
        [Parameter, SupplyParameterFromQuery(Name = "macaddress")]
        public string MACAddress { get; set; } = default!;

        protected Sufficit.Telephony.Device? Item { get; set; }

        protected Sufficit.Telephony.EndPoint? EndPoint { get; set; }

        protected string? Exception { get; set; }

        protected bool Loading { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Item == null && !string.IsNullOrWhiteSpace(MACAddress))
            {
                Loading = true;

                Item = await APIClient.Provisioning.ByMAC(MACAddress, CancellationToken.None);
                if (Item != null)
                {
                    var contextid = Item.ContextId.GetValueOrDefault();
                    if (contextid != Guid.Empty && contextid != ContextView.ContextId)
                        await ContextView.Update(contextid);

                    if (Item.ExtensionId.HasValue)
                    {
                        EndPoint = await APIClient.Telephony.EndPoint.GetEndPoint(Item.ExtensionId.Value, CancellationToken.None);
                        if (EndPoint == null)
                            Exception = $"endpoint not found: {Item.ExtensionId.Value}";
                    }
                }

                Loading = false;

                // Updating view
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            ContextView.OnChanged += OnContextViewChanged;
        }

        protected async void OnContextViewChanged(Guid? _)
            => await InvokeAsync(StateHasChanged);

        public override void Dispose(bool disposing = false)
        {
            ContextView.OnChanged -= OnContextViewChanged;
        }

        private async Task<IEnumerable<Contact>> GetContacts(string value, CancellationToken cancellationToken)
        {
            Loading = true;
            if (!string.IsNullOrWhiteSpace(value))            
                return await APIClient.Contacts.Search(value, 10, cancellationToken);
            
            Loading = false;
            return Array.Empty<Contact>();
        }

        protected async Task Remove(MouseEventArgs _)
        {
            if (Item != null)
            {
                var parameters = new DialogParameters();
                try
                {
                    await APIClient.Provisioning.Delete(Item.MACAddress, CancellationToken.None);

                    parameters.Add("Content", "Removido com sucesso.");
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
        protected async Task Clear(MouseEventArgs _)
        {
            if (Item != null)
            {
                var parameters = new DialogParameters();
                try
                {
                    Item.ContextId = null;
                    Item.ExtensionId = null;

                    await APIClient.Provisioning.Update(Item, CancellationToken.None);

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

        protected async Task Save(MouseEventArgs _)
        {
            if (Item == null)
            {
                if (MACAddress.IsValidMACBytes())
                    Item = new Sufficit.Telephony.Device() { MACAddress = MACAddress };
                else throw new Exception($"Invalid MAC Address: {MACAddress}");
            }       

            var parameters = new DialogParameters();  
            try
            {
                Item.ContextId = ContextView.ContextId;
                Item.ExtensionId = EndPoint?.Id;

                await APIClient.Provisioning.Update(Item, CancellationToken.None);

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
