using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Contacts
{
    public partial class DashBoard : BasePageComponent
    {
        public const string RouteParameter = "pages/contacts/dashboard";

        protected override string Title => "DashBoard";

        protected override string Description => "Contacts Manager";

        public uint PageSize { get; set; } = 25;


        [EditorRequired]
        protected MudTable<ContactWithAttributes>? Table { get; set; } = default!;

        private CancellationTokenSource? TokenSource;

        protected IEnumerable<ContactWithAttributes> DataItems { get; set; } = Array.Empty<ContactWithAttributes>();

        [Parameter]
        public uint Limit { get; set; } = 5;

        /// <summary>
        /// Set minimum length to start a server request
        /// </summary>
        [Parameter]
        public uint Minimum { get; set; } = 4;

        [Parameter]
        public uint TimeOut { get; set; } = 10000;

        [Parameter]
        public string? Filter { get; set; }

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private ExceptionControlService Exceptions { get; set; } = default!;

        [EditorRequired]
        [CascadingParameter]        
        protected UserPrincipal User { get; set; } = default!;
        
        [Parameter]
        public EventCallback<ContactWithAttributes> SelectedItemChanged { get; set; }

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        protected IEnumerable<Sufficit.Contacts.Contact>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            // await GetItems(default!);
        }

        protected void OnTextChanged(string? value)
        {
            Filter = value;
            DataBind();
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

        protected async Task<TableData<ContactWithAttributes>> GetData(TableState _)
        {
            // only filter if text is set
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var filter = Filter.Trim().ToLowerInvariant();
                if (filter.Length >= Minimum)
                {
                    if (TokenSource != null)
                        TokenSource.Cancel(false);

                    var parameters = new ContactSearchParameters()
                    {
                        Keys = new HashSet<string> { 
                            Attributes.Title, 
                            Attributes.EMail, 
                            Attributes.Phone, 
                            Attributes.Cellular 
                        },
                        Value = new TextFilterWithKeys(Filter) { 
                            ExactMatch = false,
                        },
                        Limit = Limit,
                    };

                    TokenSource = new CancellationTokenSource((int)TimeOut);
                    try
                    {
                        DataItems = await APIClient.Contacts.Search(parameters, TokenSource.Token);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        Snackbar.Add(ex.Message, Severity.Error);
                    }
                }
            }

            return new TableData<ContactWithAttributes>() { Items = DataItems };
        }

        protected string? GetPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var numbers = new string(phone.Where(Char.IsDigit).ToArray());
            return Sufficit.Telephony.Utils.FormatToE164Semantic(numbers);
        }

        protected async Task GetItems(CancellationToken cancellationToken)
        {
            IsLoading = true;
            try
            {
                var parameters = new ContactSearchParameters
                {
                    ContextId = User.GetUserId(),
                    Limit = PageSize
                };

                await InvokeAsync(StateHasChanged);
                Items = await APIClient.Contacts.Search(parameters, cancellationToken);
            }
            catch (Exception ex){ Exceptions.Append(User.GetUserId(), ex); }
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
