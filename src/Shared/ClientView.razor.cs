using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Identity;
using System;
using System.Threading.Tasks;
using static Sufficit.Contacts.Constants;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ClientView : ComponentBase, IDisposable
    {
        [Inject]
        IContextView ContextView { get; set; } = default!;

        /// <summary>
        /// Checking if user is authenticated to get client document
        /// </summary>
        [CascadingParameter]
        protected UserPrincipal? User { get; set; }

        [Inject] 
        IDialogService Dialog { get; set; } = default!;

        public string? ContextTitle { get; internal set; } = UNTITLED;

        public string? ContextDocument { get; internal set; }

        public bool Collapsed { get; internal set; }

        private bool Rendered { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {            
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            Rendered = true;
            if (ContextView.ContextId.HasValue)
                Update(ContextView.ContextId);

            ContextView.OnChanged += Update;           
        }

        private async void Update(Guid? value)
        {
            if (value.GetValueOrDefault() != Guid.Empty)
            {
                ContextTitle = await ContextView.GetTitle();
                if (User?.Identity?.IsAuthenticated ?? false)
                    ContextDocument = await ContextView.GetDocument();   
            }

            if (Rendered)
                await InvokeAsync(StateHasChanged);
        }

        protected void ChooseClicked(MouseEventArgs _)
        {
            Dialog.Show<ContextFilterDialog>();
        }

        protected string ProfileLink => "https://www.sufficit.com.br/relacionamento/contato?idobjeto=" + ContextView.ContextId;

        void IDisposable.Dispose()
        {
            ContextView.OnChanged -= Update;
        }
    }
}
