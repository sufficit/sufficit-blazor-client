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

namespace Sufficit.Blazor.Client.Pages.Telephony.IVR
{
    [Authorize(Roles = TelephonyRole.NormalizedName)]
    public partial class Object : TelephonyBasePageComponent, IDisposable, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/ivr";

        public const string Title = "IVR";
        protected override string Description => "Opções do painel de eventos";

        #endregion

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        protected IContextView ContextView { get; set; } = default!;

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery(Name = IVRSearchParameters.IVRID)]
        public Guid IVRId { get; set; } = default!;

        protected Sufficit.Telephony.IVR? Item { get; set; }

        protected ICollection<Sufficit.Telephony.IVROption>? IVROptions { get; set; }

        private async void ContextViewChanged(Guid? value)
        {
            var contextid = value.GetValueOrDefault();
            if (contextid != Guid.Empty)
            {
                if (Item == null || Item.IdContext != contextid)
                {
                    Item = new Sufficit.Telephony.IVR
                    {
                        Id = Guid.NewGuid(),
                        IdContext = contextid
                    };

                    IVRId = Item.Id;
                }
            } 
            else 
            {
                Item = null;
                IVRId = Guid.Empty;
            }                        

            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // should create a new item
            if (Item == null)
            {
                if (IVRId != Guid.Empty)
                {
                    Item = await APIClient.Telephony.IVR.Find(IVRId);
                    if (Item != null)
                    {
                        IVROptions = (await APIClient.Telephony.IVR.GetOptions(Item.Id)).ToList();

                        // updating context view from ivr context
                        if (Item.IdContext != ContextView.ContextId)
                        {
                            // changing before render, to avoid 
                            await ContextView.Update(Item.IdContext, false, true);
                        }
                    }
                    else throw new Exception($"Item not found: {IVRId}");

                    // Updating view
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    var contextid = ContextView.ContextId.GetValueOrDefault();
                    if (contextid != Guid.Empty)
                    {
                        Item = new Sufficit.Telephony.IVR
                        {
                            Id = Guid.NewGuid(),
                            IdContext = contextid
                        };
                    }
                }
            }            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

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
                    await APIClient.Telephony.IVR.Update(Item.Id, IVROptions);

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
