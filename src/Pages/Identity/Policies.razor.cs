using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Blazor.Client.Shared.Tables;
using Sufficit.Identity;
using Sufficit.Client;
using MudBlazor;
using Sufficit.Blazor.Client.Shared;
using AsterNET;
using Sufficit.Blazor.Components;

namespace Sufficit.Blazor.Client.Pages.Identity
{
    [Authorize(Roles = "manager")]
    public partial class Policies : BasePageComponent
    {
        protected override string Title => "Políticas de usuário";

        protected override string Description => "Diretivas de acesso";

        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Inject]
        protected ILogger<Policies> Logger { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        private string? Status { get; set; }

        protected bool Disabled => Status == null || Status.ToLowerInvariant() != "healthy";

        private Sufficit.Identity.Client.User? UserSelected { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "filter")]
        public string? QueryFilter { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid UserID { get; set; }

        [EditorRequired]
        protected UserDirectivesTable? UDTable { get; set; }

        protected UserSearchTable? UserSearchTableReference { get; set; } = default;

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

            if (UserID != Guid.Empty)
            {
                var user = await BIService.Identity.Users.GetUserAsync(UserID);
                if (user != null)
                    OnUserSelect(user);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var statusPrevious = Status;
            try
            {
                Status = (await BIService.Identity.Health())?.Status;
            }
            catch (Exception ex)
            {
                Status = "Não disponível";
                Logger.LogError(ex, statusPrevious, ex.Message);
            }

            if (statusPrevious != Status) 
                await InvokeAsync(StateHasChanged);
        }

        protected void OnDirectiveAdded()
        { 
            if (UDTable != null)
                UDTable.DataBind();
        }

        public async void OnUserSelect(User selected)
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

            var parameters = new DialogParameters();
            parameters.Add("Question", $"Tem certeza que deseja remover o usuário, {selected.EMail} ?");
            parameters.Add("Observation", "Essa ação não poderá ser desfeita");
            var dialogReferense = DialogService.Show<ConfirmDialog>("Remover permanentemente", parameters, options);

            var result = await dialogReferense.Result;
            if (!result.Canceled)
            {
                try
                {
                    await BIService.RemoveUser(selected, cancellationToken);

                    UserSelected = null;
                    UserSearchTableReference?.DataBind();
                    await InvokeAsync(StateHasChanged);

                    Logger.LogInformation("user removed: {0}", selected.EMail);
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
