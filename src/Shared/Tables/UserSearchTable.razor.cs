using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
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

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected ILogger<UserSearchTable> Logger { get; set; } = default!;

        [Parameter]
        public uint Limit { get; set; } = 5;

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public uint TimeOut { get; set; } = 4000;

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

                if (Table != null)
                {
                    await Table.ReloadServerData();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        /// <summary>
        /// Reload server data
        /// </summary>
        public async void DataBind()
        {
            if (Table != null)
            {
                await Table.ReloadServerData();
                await InvokeAsync(StateHasChanged);
            }
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

        protected async Task ResendEMailConfirmation(User selected)
        {
            try
            {
                await BIService.Identity.ResendEMailConfirmation(selected.EMail!);
                Snackbar.Add("Pronto ! E-Mail de confirmação re-enviado", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        protected async Task PasswordReset(User selected)
        {
            var options = new DialogOptions()
            {
                CloseButton = false,
                CloseOnEscapeKey = true,
                Position = DialogPosition.TopCenter,
                FullWidth = true,
            };

            var parameters = new DialogParameters();
            parameters.Add("Question", $"Tem certeza que deseja redefinir a senha para o usuário, {selected.EMail} ?");
            var dialogReferense = DialogService.Show<ConfirmDialog>("Redefinir senha", parameters, options);

            var result = await dialogReferense.Result;
            if (!result.Canceled)
            {
                try
                {
                    var newPassword = await BIService.ResetPassword(selected.ID, default);
                    Snackbar.Add($"Pronto ! Nova senha temporária: {newPassword}", Severity.Success);
                } 
                catch(Exception ex)
                {
                    Logger.LogError(ex, "error on password reset");
                    Snackbar.Add("Deu ruim em algo, tente mais tarde.", Severity.Error);
                }
            }
        }
    }
}
