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

        private string? Status { get; set; }

        private Sufficit.Identity.Client.User? UserSelected { get; set; }

        private IEnumerable<UserClaimPolicy>? UserPolicies { get; set; }

        private string? UserClaimsMessage { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "filter")]
        public string? QueryFilter { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid UserID { get; set; }

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
            Status = (await BIService.Identity.Health())?.Status;
            if (statusPrevious != Status) 
                await InvokeAsync(StateHasChanged);
        }

        private GetUsersResponse? UsersResponse { get; set; }


        

        protected async Task ReloadSearch()
        {
           // await ValueChanged(TextInputReference?.Value);
        }

        public async void OnUserSelect(User selected)
        {
            if (UserSelected != selected)
            {
                UserSelected = selected;
                if (UserSelected != null)
                {
                    UserPolicies = await BIService.GetUserPolicies(UserSelected, default);

                    //refreshing ui
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected async Task ConfirmPasswordReset(User selected, CancellationToken cancellationToken = default)
        {
            /*
            var alert = new SweetAlert() { 
                TimerProgressBar = true, 
                Timer = 5000, 
                Title = "Redefinir senha ?",
                Text = $"{ selected.EMail }",
                Icon = "question",
                ShowDenyButton = true,
                DenyButtonText = "Não",
                ConfirmButtonText = "Continuar"
            };

            var Swal = UIService.SweetAlerts;
            var result = await Swal.Fire(alert, cancellationToken);
            if (result != null)
            {
                if (result.IsConfirmed)
                {
                    var newPassword = await BIService.ResetPassword(selected.ID, cancellationToken);
                    SweetAlert saConfirm;
                    if (!string.IsNullOrWhiteSpace(newPassword)) {
                        saConfirm = new SweetAlert()
                        {
                            Title = "Pronto !",
                            Text = $"nova senha temporária: { newPassword }",
                            Icon = "success"
                        };
                    } else {
                        saConfirm = new SweetAlert()
                        {
                            Title = "Oops...",
                            Text = "Deu ruim em algo, tente mais tarde.",
                            Icon = "error"
                        };
                    }            

                    await Swal.Fire(saConfirm);
                }
            }
            */
        }

        protected async Task<string> GetContactTitle(Guid idcontact, CancellationToken cancellationToken = default)
        {
            if (idcontact == Guid.Empty) return "* Todos";
            /*
            var contact = await BIService.GetContact(idcontact, cancellationToken);
            if (contact == null) return string.Empty;
            return contact.Title ?? "* Desconhecido";
            */
            return "* Desconhecido";
        }

        /*
        protected SearchInput? InputDirective { get; set; } = default!;

        protected SearchInput? InputContext { get; set; } = default!;
        */

        
        protected async void OnDelClick(int? id)
        {
            if (id.HasValue)
            {
                if (UserSelected != null)
                {
                    await BIService.RemoveUserPolicy(UserSelected, id.Value, default);
                    OnUserSelect(UserSelected);
                }
                else throw new Exception("no user selected");
            }
            else throw new Exception("id not recognized");
        }

        protected async void OnUserDelClick(User selected, CancellationToken cancellationToken = default)
        {
            /*
            var alert = new SweetAlert()
            {
                TimerProgressBar = true,
                Timer = 5000,
                Title = "Tem certeza que deseja remover o usuário ?",
                Text = $"{ selected.EMail }",
                Icon = "question",
                ShowDenyButton = true,
                DenyButtonText = "Não",
                ConfirmButtonText = "Continuar"
            };

            var Swal = UIService.SweetAlerts;
            var result = await Swal.Fire(alert, cancellationToken);
            if (result != null)
            {
                if (result.IsConfirmed)
                {
                    try
                    {
                        await BIService.RemoveUser(selected, cancellationToken);
                        await SelectUser(null, cancellationToken);
                        await ReloadSearch();

                        SweetAlert saConfirm = new SweetAlert()
                        {
                            Title = "Pronto !",
                            Icon = "success"
                        }; 
                        await Swal.Fire(saConfirm);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "error on remove user");
                        SweetAlert saConfirm = new SweetAlert()
                        {
                            Title = "Oops...",
                            Text = "Deu ruim em algo, tente mais tarde.",
                            Icon = "error"
                        };
                        await Swal.Fire(saConfirm);
                    }
                }
            }
            */
        }
    }
}
