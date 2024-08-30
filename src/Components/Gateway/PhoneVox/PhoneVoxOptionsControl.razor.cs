using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Gateway.PhoneVox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Gateway.PhoneVox
{
    [Authorize(Roles = "telephony")]
    public partial class PhoneVoxOptionsControl : ComponentBase, IDisposable
    {
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

        protected PhoneVoxOptions Options { get; set; } = default!;


        protected static bool IsVisibleOS(string system)
        {
            return system switch
            {
                PhoneVoxOptions.SYSTEMIXC => false,
                _ => true
            };
        }

        protected static string GetTitleForOccurrence(string system)
        {
            return system switch
            {
                PhoneVoxOptions.SYSTEMIXC => "Assuntos",
                _ => "Ocorrência"
            };
        }

        protected static string GetTitleForSection(string system)
        {
            return system switch
            {
                PhoneVoxOptions.SYSTEMSGP => "Setores",
                PhoneVoxOptions.SYSTEMIXC => "Departamentos",
                _ => "Seção"
            };
        }

        #region Destinations

        protected PhoneVoxDestination Other { get; set; }
            = new PhoneVoxDestination() { Title = PhoneVoxDestination.OTHER };

        protected PhoneVoxDestination Comercial { get; set; }
            = new PhoneVoxDestination() { Title = PhoneVoxDestination.COMERCIAL };

        protected PhoneVoxDestination Finance { get; set; }
            = new PhoneVoxDestination() { Title = PhoneVoxDestination.FINANCE };

        protected PhoneVoxDestination Support { get; set; }
            = new PhoneVoxDestination() { Title = PhoneVoxDestination.SUPPORT };

        #endregion

        protected void Defaults()
        {
            // default empty settings
            Options = new PhoneVoxGateway();
            
            if (GatewayId != Guid.Empty)
                Options.Id = GatewayId;

            if (ContextView.ContextId.HasValue)
                Options.ContextId = ContextView.ContextId.Value;

            Options.Server.Title = "default";
            Options.IdOccurrence = new();
            Options.IdOS = new();
            Options.IdSection = new();
        }

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
            IsLoading = true;

            // clearing form
            Defaults();

            await InvokeAsync(StateHasChanged);

            if (GatewayId != Guid.Empty) 
            { 
                var options = await APIClient.Gateway.PhoneVox.GetById(GatewayId);
                if (options != null)
                {
                    Options = options;
                    IsDraft = false;

                    var destinations = (await APIClient.Gateway.PhoneVox.GetDestinations(GatewayId, default))?.ToList();
                    
                    Other = destinations?.FirstOrDefault(s => s.Title == PhoneVoxDestination.OTHER) ?? new PhoneVoxDestination() { Title = PhoneVoxDestination.OTHER };
                    Comercial = destinations?.FirstOrDefault(s => s.Title == PhoneVoxDestination.COMERCIAL) ?? new PhoneVoxDestination() { Title = PhoneVoxDestination.COMERCIAL };
                    Finance = destinations?.FirstOrDefault(s => s.Title == PhoneVoxDestination.FINANCE) ?? new PhoneVoxDestination() { Title = PhoneVoxDestination.FINANCE };
                    Support = destinations?.FirstOrDefault(s => s.Title == PhoneVoxDestination.SUPPORT) ?? new PhoneVoxDestination() { Title = PhoneVoxDestination.SUPPORT };                    
                } 
                else
                {
                    IsDraft = true;
                }
            } 

            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        protected IEnumerable<PhoneVoxDestination> GetDestinations()
        {
            var destinations = new List<PhoneVoxDestination>();
            if (!string.IsNullOrWhiteSpace(Other.Asterisk)) destinations.Add(Other);
            if (!string.IsNullOrWhiteSpace(Comercial.Asterisk)) destinations.Add(Comercial);
            if (!string.IsNullOrWhiteSpace(Finance.Asterisk)) destinations.Add(Finance);
            if (!string.IsNullOrWhiteSpace(Support.Asterisk)) destinations.Add(Support);
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
                    await APIClient.Gateway.PhoneVox.Update(Options);

                    // updating options
                    await APIClient.Gateway.PhoneVox.Update(Options.Id, destinations);

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
                await APIClient.Gateway.PhoneVox.Delete(Options.Id);
                Cancel();
            }
            catch (Exception ex)
            {
                var parameters = new DialogParameters();
                parameters.Add("Ex", ex);
                parameters.Add("Content", "Falha ao remover.");
                DialogService.Show<StatusDialog>("Falha !", parameters);
            }
        }

        protected void GoToHistory(MouseEventArgs _)
        {
            if (Options != null)
            {
                var url = Pages.Logging.Events.GetLink(Options.Id, null, nameof(PhoneVoxGateway));
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
