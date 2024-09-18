using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Contacts
{
    public partial class DashBoard : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/contacts/dashboard";

        protected override string? Area => "Contatos";
        public const string Title = "DashBoard";
        protected override string Description => "Contacts Manager";

        #endregion

        public uint PageSize { get; set; } = 25;


        [EditorRequired]
        protected MudTable<ContactWithAttributes> Table { get; set; } = default!;

        protected IEnumerable<ContactWithAttributes> DataItems { get; set; } = Array.Empty<ContactWithAttributes>();

        [Parameter]
        public uint Limit { get; set; } = 20;

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
        protected APIClientService APIClient { get; set; } = default!;
                
        [Parameter]
        public EventCallback<ContactWithAttributes> SelectedItemChanged { get; set; }

        protected string? GetInfoLink(Guid id)
            => $"/{Object.RouteParameter}?contactid={id:N}";

        /// <summary>
        ///     Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        protected IEnumerable<Sufficit.Contacts.Contact>? Items { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            DataBind();
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

        /// <summary>
        ///     Getting data cancellation token source
        /// </summary>
        private CancellationTokenSource? TokenSource;

        protected async Task<TableData<ContactWithAttributes>> GetData(TableState _, CancellationToken cancellationToken)
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
                            Attributes.Cellular,
                            Attributes.Document
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
    }
}
