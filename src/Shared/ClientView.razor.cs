using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ClientView : ComponentBase, IDisposable
    {
        [Inject]
        IContextView View { get; set; } = default!;

        /// <summary>
        /// Checking if user is authenticated to get client document
        /// </summary>
        [Inject]
        IAuthenticationStateProvider StateProvider { get; set; } = default!;

        [Inject] 
        IDialogService Dialog { get; set; } = default!;

        public string? ContextTitle { get; internal set; } = "Desconhecido";

        public string? ContextDocument { get; internal set; }

        public bool Collapsed { get; internal set; }

        private bool Rendered { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            //Collapsed = SBService.Collapsed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {            
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            Rendered = true;
            if (View.ContextId != Guid.Empty)
                Update(View.ContextId);

            View.OnChanged += Update;
            //SBService.OnCollapsing += SBServiceToggle;            
        }

        private async void SBServiceToggle(bool collapsed)
        {
            if(Collapsed != collapsed)
            {
                Collapsed = collapsed;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async void Update(Guid obj)
        {
            if (obj != Guid.Empty)
            {
                ContextTitle = await View.GetTitle();
                
                var state = await StateProvider.GetAuthenticationStateAsync();
                if (state.User?.Identity?.IsAuthenticated ?? false)
                    ContextDocument = await View.GetDocument();                
            }

            if(Rendered)
                await InvokeAsync(StateHasChanged);
        }

        protected void ChooseClicked(MouseEventArgs _)
        {
            Dialog.Show<ContextFilterDialog>();
        }

        protected string ProfileLink => "https://www.sufficit.com.br/relacionamento/contato?idobjeto=" + View.ContextId;

        void IDisposable.Dispose()
        {
            View.OnChanged -= Update;
           // SBService.OnCollapsing -= SBServiceToggle;
        }
    }
}
