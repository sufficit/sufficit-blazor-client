using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Charts;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Tables
{
    public partial class UserSearchTable : ComponentBase
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
        public EventCallback<User> SelectedItemChanged { get; set; }

        [EditorRequired]
        protected MudTable<User>? Table { get; set; } = default!;

        private CancellationTokenSource? TokenSource;

        protected IEnumerable<User> DataItems { get; set; } = Array.Empty<User>();

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

        protected async Task<TableData<User>> GetData(TableState _)
        {
            // only filter if text is set
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filter = Filter.Trim().ToLowerInvariant();
                if (filter.Length >= Minimum)
                {
                    if (TokenSource != null)
                        TokenSource.Cancel(false);

                    TokenSource = new CancellationTokenSource((int)TimeOut);
                    try
                    {
                        var response = await BIService.Identity.Users.GetUsersAsync(Filter, 1, (int)Limit, TokenSource.Token);
                        DataItems = response?.Users ?? Array.Empty<User>();
                    }
                    catch (TaskCanceledException) { DataItems = Array.Empty<User>(); }
                }
                else DataItems = Array.Empty<User>();
            }
            else DataItems = Array.Empty<User>();

            return new TableData<User>() { Items = DataItems };
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
