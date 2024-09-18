using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Client;
using Sufficit.Gateway.ReceitaNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Gateway.ReceitaNet
{
    [Authorize(Roles = "telephony")]
    public partial class ReceitaNetOptionsControl : ComponentBase, IDisposable
    {
        public const string Description = "Gateway | ReceitaNet (Software para Provedores)";

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public Guid GatewayId { get; set; }

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }

        protected bool IsDraft { get; set; }
                
        private void OnContextViewChanged(Guid? _)
            => Cancel();

        public RNOptions Options { get; set; } = default!;

        #region Destinations

        protected RNDestination Fail { get; set; }
            = new RNDestination() { Title = RNDestination.FAIL };

        protected RNDestination Hangup { get; set; }
            = new RNDestination() { Title = RNDestination.HANGUP };

        protected RNDestination Solicited { get; set; }
            = new RNDestination() { Title = RNDestination.SOLICITED };

        protected RNDestination Connected { get; set; }
            = new RNDestination() { Title = RNDestination.CONNECTED };

        protected RNDestination Unknown { get; set; }
            = new RNDestination() { Title = RNDestination.UNKNOWN };

        #endregion

        protected void Defaults()
        {
            // default empty settings
            Options = new RNOptions();
            
            if (GatewayId != Guid.Empty)
                Options.Id = GatewayId;

            if (ContextView.ContextId.HasValue)
                Options.ContextId = ContextView.ContextId.Value;

            Options.Title = "default";
        }

        #region TOKENS

        protected string CurrentToken { get; set; } = default!;

        protected void OnTokenAdded (MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurrentToken))
            {
                var tokens = Options.Tokens.ToHashSet();
                tokens.Add(CurrentToken);
                Options.Tokens = tokens.ToArray();

                CurrentToken = default!;
            }            
        }

        protected void OnTokenRemoved(string token)
        {
            var tokens = Options.Tokens.ToHashSet();
            if (tokens.Remove(token))
                Options.Tokens = tokens.ToArray();            
        }

        #endregion

        protected override async Task OnParametersSetAsync()
        {
            // getting items for the first time
            await DataBind();

            // if change, get items again
            ContextView.OnChanged -= OnContextViewChanged;
            ContextView.OnChanged += OnContextViewChanged;
        }

        /// <summary>
        /// Retrieving data
        /// </summary>
        /// <returns></returns>
        protected async Task DataBind()
        {
            if (GatewayId == Guid.Empty)
            {
                IsDraft = true;
                return;
            }

            IsLoading = true;

            // clearing form
            Defaults();

            await InvokeAsync(StateHasChanged);

            if (GatewayId != Guid.Empty) 
            { 
                var options = await APIClient.Gateway.ReceitaNet.GetById(GatewayId);
                if (options != null)
                {
                    Options = options;
                    IsDraft = false;

                    var destinations = (await APIClient.Gateway.ReceitaNet.GetDestinations(GatewayId, default))?.ToList();
                    
                    Fail = destinations?.FirstOrDefault(s => s.Title == RNDestination.FAIL) ?? new RNDestination() { ID = GatewayId, Title = RNDestination.FAIL };
                    Hangup = destinations?.FirstOrDefault(s => s.Title == RNDestination.HANGUP) ?? new RNDestination() { ID = GatewayId, Title = RNDestination.HANGUP };
                    Solicited = destinations?.FirstOrDefault(s => s.Title == RNDestination.SOLICITED) ?? new RNDestination() { ID = GatewayId, Title = RNDestination.SOLICITED };
                    Connected = destinations?.FirstOrDefault(s => s.Title == RNDestination.CONNECTED) ?? new RNDestination() { ID = GatewayId, Title = RNDestination.CONNECTED };
                    Unknown = destinations?.FirstOrDefault(s => s.Title == RNDestination.UNKNOWN) ?? new RNDestination() { ID = GatewayId, Title = RNDestination.UNKNOWN };
                } 
                else
                {
                    IsDraft = true;
                }
            } 

            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        protected IEnumerable<RNDestination> GetDestinations()
        {
            var destinations = new List<RNDestination>();
            if (!string.IsNullOrWhiteSpace(Fail.Asterisk)) destinations.Add(Fail);
            if (!string.IsNullOrWhiteSpace(Hangup.Asterisk)) destinations.Add(Hangup);
            if (!string.IsNullOrWhiteSpace(Solicited.Asterisk)) destinations.Add(Solicited);
            if (!string.IsNullOrWhiteSpace(Connected.Asterisk)) destinations.Add(Connected);
            if (!string.IsNullOrWhiteSpace(Unknown.Asterisk)) destinations.Add(Unknown);
            return destinations;
        }

        protected void Cancel()
            => NavigationManager.NavigateTo<Pages.Gateway.PhoneVox>();        

        /// <summary>
        ///     Saving changes
        /// </summary>
        protected async Task Save()
        {
            var parameters = new DialogParameters();
            if (Options.ContextId == Guid.Empty)
            {
                parameters.Add("Content", "Faltando Contexto");
                DialogService.Show<StatusDialog>("Falha !", parameters);
                return;
            } 

            if (Options.Id == Guid.Empty)
            {
                parameters.Add("Content", "Faltando Id");
                DialogService.Show<StatusDialog>("Falha !", parameters);
                return;
            }

            var destinations = GetDestinations();
            if (!destinations.Any())
            {
                parameters.Add("Content", "Nenhum destino específicado");
                DialogService.Show<StatusDialog>("Falha !", parameters);
            }
            else
            {
                try
                {
                    // updating basic info
                    await APIClient.Gateway.ReceitaNet.Update(Options, default);

                    // updating options
                    await APIClient.Gateway.ReceitaNet.Update(Options.Id, destinations);

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

        protected async Task Remove()
        {
            try
            {
                await APIClient.Gateway.ReceitaNet.Delete(Options.Id);
                Cancel();
            }
            catch (Exception ex)
            {
                var parameters = new DialogParameters
                {
                    { "Ex", ex },
                    { "Content", "Falha ao remover." }
                };
                DialogService.Show<StatusDialog>("Falha !", parameters);
            }
        }

        protected void GoToHistory(MouseEventArgs _)
        {
            if (Options != null)
            {
                var url = Pages.Logging.Events.GetLink(Options.Id, null, "ReceitaNetFlow");
                NavigationManager.NavigateTo(url);
            }
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= OnContextViewChanged;
        }
    }
}
