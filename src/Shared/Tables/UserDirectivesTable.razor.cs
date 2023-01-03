using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Charts;
using Newtonsoft.Json.Linq;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Tables
{
    public partial class UserDirectivesTable : ComponentBase
    {
        [Inject]
        protected BlazorIdentityService BIService { get; set; } = default!;

        [Parameter]
        public uint Limit { get; set; } = 5;

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        [Parameter]
        public string? Filter { get; set; }

        [Parameter]
        public EventCallback<UserClaimPolicy> SelectedItemChanged { get; set; }

        [Parameter]
        [EditorRequired]
        public User? User { get; set; }

        private User? _user;
        private User? _User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    DataBind();
                }
            }
        }

        [EditorRequired]
        protected MudTable<UserClaimPolicy>? Table { get; set; } = default!;


        private CancellationTokenSource? TokenSource;

        protected IEnumerable<UserClaimPolicy> DataItems { get; set; } = Array.Empty<UserClaimPolicy>();

        protected override void OnParametersSet()
        {
            if (_User != User)            
                _User = User;            
        }

        /// <summary>
        /// Update filter parameter and reload server data
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetFilter(string? value)
        {
            if(Filter != value)
            {
                Filter = value;

                if(Table != null)
                    await Table.ReloadServerData();
            }
        }

        /// <summary>
        /// Reload server data
        /// </summary>
        public async void DataBind()
        {
            if (Table != null)            
                await Table.ReloadServerData();            
        }

        protected async Task<TableData<UserClaimPolicy>> GetData(TableState _)
        {
            if (TokenSource != null)
                TokenSource.Cancel(false);

            if (User != null)
            {
                TokenSource = new CancellationTokenSource((int)TimeOut);
                try
                {
                    var response = await BIService.GetUserPolicies(User, TokenSource.Token);
                    DataItems = response ?? Array.Empty<UserClaimPolicy>();
                }
                catch (TaskCanceledException) { DataItems = Array.Empty<UserClaimPolicy>(); }
            } else { DataItems = Array.Empty<UserClaimPolicy>(); }

            return new TableData<UserClaimPolicy>() { Items = DataItems };
        }

        protected async Task<string> GetContactTitle(Guid idcontact, CancellationToken cancellationToken = default)
        {
            if (idcontact == Guid.Empty) return "* Todos";
            
            var contact = await BIService.GetContact(idcontact, cancellationToken);
            if (contact == null) return string.Empty;
            return contact.Title ?? "* Desconhecido";
        }
        protected async void OnDelClick(int? id)
        {
            if (id.HasValue)
            {
                if (User != null)
                {
                    await BIService.RemoveUserPolicy(User, id.Value, default);
                    DataBind();
                }
                else throw new Exception("no user selected");
            }
            else throw new Exception("id not recognized");
        }

        /*
        protected async Task ValueChanged(string? searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length > 3)
            {
                UsersResponse = await BIService.Identity.Users.GetUsersAsync(searchText);
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

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                UsersResponse = null;
                UsersMessage = "Mínimo de 4 caracteres para consultar";
            }
        }
        */
    }
}
