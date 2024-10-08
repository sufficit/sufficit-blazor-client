﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Gateway.PhoneVox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Gateway.PhoneVox
{
    public partial class PhoneVoxOptionsTable : ComponentBase, IDisposable
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        private IContextView ContextView { get; set; } = default!;

        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        [Parameter]
        public EventCallback OnChanged { get; set; }
                
        protected MudTable<PhoneVoxOptions> Table { get; set; } = default!;

        private CancellationTokenSource? TokenSource;

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
                return;
            
            ContextView.OnChanged -= OnContextViewChanged;
            ContextView.OnChanged += OnContextViewChanged;

            if (Table != null)
                Table.Context.TableStateHasChanged = async () => await OnChanged.InvokeAsync();
        }

        private async void OnContextViewChanged(Guid? _)
            => await DataBind();

        /// <summary>
        ///     Reload server data
        /// </summary>
        public async ValueTask DataBind()
        {
            if (Table != null)            
                await Table.ReloadServerData();
        }

        protected TableData<PhoneVoxOptions>? LastData;

        protected async Task<TableData<PhoneVoxOptions>> TableServerData (TableState state, CancellationToken cancellationToken)
        {            
            LastData ??= new TableData<PhoneVoxOptions>()
            {
                Items = Array.Empty<PhoneVoxOptions>()
            };

            if (!ContextView.ContextId.HasValue)
                return LastData;

            if (TokenSource != null)
                TokenSource.Cancel(false);

            var tss = new CancellationTokenSource((int)TimeOut);
            var ts = TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, tss.Token);
            try
            {          
                var result = await APIClient.Gateway.PhoneVox.GetByContextId(ContextView.ContextId.Value, cancellationToken);
                LastData.Items = result;
            }
            catch (OperationCanceledException) { }
            return LastData;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= OnContextViewChanged;
        }
    }
}
