using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Sufficit.Identity.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Blazor.Client.Shared.Tables;
using MudBlazor;
using Sufficit.Blazor.Client.Shared;
using Sufficit.Blazor.Components;
using Sufficit.Identity;

namespace Sufficit.Blazor.Client.Pages.Identity
{
    [Authorize(Roles = ManagerRole.NormalizedName)]
    public partial class Policies : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;    
        public const string RouteParameter = "/pages/identity/policies";

        public const string Title = "Políticas de usuário";
        protected override string Description => "Diretivas de acesso";

        #endregion

        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Inject]
        protected ILogger<Policies> Logger { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;


        protected bool Disabled => Status == null || Status.ToLowerInvariant() != "healthy";

        private Sufficit.Identity.Client.User? UserSelected { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "filter")]
        public string? QueryFilter { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid UserId { get; set; }

        [EditorRequired]
        protected UserDirectivesTable? UDTable { get; set; }

        [EditorRequired]
        protected MudTextField<string>? FilterTextFiled { get; set; }

        protected UserSearchTable? UserSearchTableReference { get; set; } = default;

        private string Status
            => Health?.Status ?? "Não disponível";

        protected HealthResponse? Health { get; set; }

        protected async void OnTextChanged(string? value)
        {            
            if (UserSearchTableReference != null)
                await UserSearchTableReference.SetFilter(value);            
        }

        protected string? Filter { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (QueryFilter != null)
                Filter = QueryFilter;            

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                var statusPrevious = Status;
                Health = await BIService.Identity.Health();

                if (statusPrevious != Status)
                    await InvokeAsync(StateHasChanged);

                if (string.IsNullOrWhiteSpace(Filter) && FilterTextFiled != null)
                    await FilterTextFiled.FocusAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "error on get status");
                Snackbar.Add("Deu ruim ao verificar estado do servidor", Severity.Error);
            }

            try
            {
                if (UserId != Guid.Empty)
                {
                    var user = await BIService.Identity.Users.GetUserAsync(UserId);
                    if (user != null)
                        OnUserSelect(user);
                }
            } catch (Exception ex) {
                Logger.LogError(ex, "error on searching for user: {0}", UserId);
                Snackbar.Add("Deu ruim em algo, tente mais tarde.", Severity.Error);
            }       
        }

        protected void OnDirectiveAdded()
        { 
            if (UDTable != null)
                UDTable.DataBind();
        }

        public async void OnUserSelect (User selected)
        {
            if (UserSelected != selected)
            {
                UserSelected = selected;
                await InvokeAsync(StateHasChanged);
            }
        }
               
        protected async void OnUserDelClick(User selected, CancellationToken cancellationToken = default)
        {
            var options = new DialogOptions()
            {
                CloseButton = false,
                CloseOnEscapeKey = true,
                Position = DialogPosition.TopCenter,
                FullWidth = true,
            };

            var parameters = new DialogParameters
            {
                { "Question", $"Tem certeza que deseja remover o usuário, {selected.EMail} ?" },
                { "Observation", "Essa ação não poderá ser desfeita" }
            };

            var dialogReferense = DialogService.Show<ConfirmDialog>("Remover permanentemente", parameters, options);

            var result = await dialogReferense.Result;
            if (result != null && !result.Canceled)
            {
                try
                {
                    await BIService.RemoveUser(selected, cancellationToken);

                    UserSelected = null;
                    UserSearchTableReference?.DataBind();
                    await InvokeAsync(StateHasChanged);

                    Logger.LogInformation("user removed: {email}", selected.EMail);
                    Snackbar.Add("Pronto ! Usuário removido com sucesso", Severity.Success);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "error on remove user");
                    Snackbar.Add("Deu ruim em algo, tente mais tarde.", Severity.Error);
                }
            }
        }
    }
}
