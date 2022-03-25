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
        private BlazorIdentityService BIService { get; set; }

        [Inject]
        public BlazorUIMaterialService UIService { get; set; }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!string.IsNullOrWhiteSpace(Filter))
                textInput?.Update(Filter, true);

            Status = (await BIService.Identity.Health())?.Status;
            await base.OnAfterRenderAsync(firstRender);
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
            }
            else
            {
                UsersResponse = null;
                UsersMessage = "Mínimo de 4 caracteres para consultar";
            }
        }

        protected async Task SelectUser(User selected, CancellationToken cancellationToken)
        {
            UserSelected = selected;
            UserPolicies = await BIService.GetUserPolicies(UserSelected, cancellationToken);
            StateHasChanged();
        }

        protected async Task ConfirmPasswordReset(User selected, CancellationToken cancellationToken)
        {
            var alert = new SweetAlert() { 
                TimerProgressBar = true, 
                Timer = 5000, 
                Title = "Redefinir senha ?",
                Text = $"{ selected.EMail }",
                Icon = "question",
                ShowDenyButton = true 
            };

            var Swal = UIService.SweetAlerts;
            var result = await Swal.Fire(alert);
            if (result != null)
            {
                if (result.IsConfirmed)
                {
                    var newPassword = await BIService.ResetPassword(selected.ID);
                    SweetAlert saConfirm;
                    if (!string.IsNullOrWhiteSpace(newPassword)) {
                        saConfirm = new SweetAlert()
                        {
                            TimerProgressBar = true,
                            Timer = 3500,
                            Title = "Pronto !",
                            Text = $"nova senha temporária: { newPassword }",
                            Icon = "success"
                        };
                    } else {
                        saConfirm = new SweetAlert()
                        {
                            TimerProgressBar = true,
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

        protected async void OnAddClick(MouseEventArgs e)
        {
            await BIService.UpdateUserPolicy(UserSelected, InputDirective.Value, Guid.Parse(InputContext.Value), default);
            await SelectUser(UserSelected, default);

            /*
            {
              "userId": "095132cd-b1c4-4043-ae87-0a59cf2e0569",
              "claimType": "directive",
              "claimValue": "audioupdate:095132cd-b1c4-4043-ae87-0a59cf2e0569"
            } 
            */
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
    }
}
