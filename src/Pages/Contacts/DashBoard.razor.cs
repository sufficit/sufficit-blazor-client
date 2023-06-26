﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
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

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private ExceptionControlService Exceptions { get; set; } = default!;

        [EditorRequired]
        [CascadingParameter]
        protected UserPrincipal User { get; set; } = default!;

        /// <summary>
        /// Used to show loading messages
        /// </summary>
        protected bool IsLoading { get; set; }
                
        protected IEnumerable<Sufficit.Contacts.Contact>? Items { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            await GetItems(default!);
        }

        protected async Task GetItems(CancellationToken cancellationToken)
        {
            IsLoading = true;
            try
            {
                var parameters = new ContactSearchParameters();
                parameters.ContextId = User.GetUserId();
                parameters.Limit = PageSize;

                await InvokeAsync(StateHasChanged);
                Items = await APIClient.Contact.Search(parameters, cancellationToken);
            }
            catch (Exception ex){ Exceptions.Append(User.GetUserId(), ex); }
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}