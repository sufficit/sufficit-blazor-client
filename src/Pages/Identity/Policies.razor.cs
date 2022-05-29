using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Sufficit.Blazor.Client.Services;
using Sufficit.Blazor.UI.Material;
using Sufficit.Blazor.UI.Material.Components;
using Sufficit.Blazor.UI.Material.Services;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Identity
{
    [Authorize]
    public partial class Policies : BasePageComponent
    {
        protected override string Title => "Políticas de usuário";

        protected override string Description => "Diretivas de acesso";

        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Inject]
        protected BlazorUIMaterialService UIService { get; set; } = default!;

        [Inject]
        protected ILogger<Policies> Logger { get; set; } = default!;

        private string Status { get; set; }

        private GetUsersResponse UsersResponse { get; set; }

        private string UsersMessage { get; set; }

        private Sufficit.Identity.Client.User UserSelected { get; set; }

        private IEnumerable<UserClaimPolicy> UserPolicies { get; set; }

        private string UserClaimsMessage { get; set; }

        public TextInput textInput { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string Filter { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid UserID { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (UserID != Guid.Empty)
            {
                var user = await BIService.Identity.Users.GetUserAsync(UserID);
                if (user != null)
                    await SelectUser(user);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                await textInput?.Update(Filter, true);
                textInput.SetDisabled(true);
            }

            var statusPrevious = Status;
            Status = (await BIService.Identity.Health())?.Status;
            if (statusPrevious != Status) StateHasChanged();
        }

        protected async Task ValueChanged(ChangeEventArgs args)
        {
            var searchText = args.Value.ToString();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length > 3)
            {
                UsersResponse = await BIService.Identity.Users.GetUsersAsync(args.Value.ToString());
                if (UsersResponse == null)
                {
                    UsersResponse = null;
                    UsersMessage = "Problema na consulta";
                }
                else if (UsersResponse.Users == null || !UsersResponse.Users.Any())
                {
                    UsersResponse = null;
                    UsersMessage = "Nenhum resultado encontrado";
                }
                    
                StateHasChanged();                
            }
            else
            {
                UsersResponse = null;
                UsersMessage = "Mínimo de 4 caracteres para consultar";
            }
        }

        protected async Task ReloadSearch()
        {
            await ValueChanged(new ChangeEventArgs() { Value = textInput.Value });
        }

        protected async Task SelectUser(User selected, CancellationToken cancellationToken = default)
        {
            UserSelected = selected;
            if(UserSelected != null)
                UserPolicies = await BIService.GetUserPolicies(UserSelected, cancellationToken);

            StateHasChanged();
        }

        protected async Task ConfirmPasswordReset(User selected, CancellationToken cancellationToken = default)
        {
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
        }

        protected async Task<string> GetContactTitle(Guid idcontact, CancellationToken cancellationToken = default)
        {
            if (idcontact == Guid.Empty) return "* Todos";

            var contact = await BIService.GetContact(idcontact, cancellationToken);
            if (contact == null) return string.Empty;
            return contact.Title;
        }

        protected SearchInput InputDirective { get; set; }

        protected SearchInput InputContext { get; set; }

        protected async Task OnAddClick(MouseEventArgs e)
        {
            var user = UserSelected;
            var directive = InputDirective.Value;
            var context = InputContext.Value;

            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(directive))
                {
                    if (!string.IsNullOrWhiteSpace(context))
                    {
                        if (Guid.TryParse(context, out Guid idcontext))
                        {
                            await BIService.UpdateUserPolicy(user, directive, idcontext, default);
                            await SelectUser(UserSelected, default);
                        }
                        else
                        {
                            throw new FormValidationException("Contexto inválido.");
                        }
                    }
                    else
                    {
                        throw new FormValidationException("Contexto em branco.");
                    }
                }
                else
                {
                    throw new FormValidationException("Diretiva em branco.");
                }
            } 
            else
            {
                throw new FormValidationException("Usuário não selecionado");
            }
        }

        protected async void OnDelClick(int? id)
        {
            if (id.HasValue)
            {
                await BIService.RemoveUserPolicy(UserSelected, id.Value, default);
                await SelectUser(UserSelected, default);
            }
            /*
            {
              "userId": "095132cd-b1c4-4043-ae87-0a59cf2e0569",
              "claimType": "directive",
              "claimValue": "audioupdate:095132cd-b1c4-4043-ae87-0a59cf2e0569"
            } 
            */
        }

        protected async void OnUserDelClick(User selected, CancellationToken cancellationToken = default)
        {
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
        }
    }
}
