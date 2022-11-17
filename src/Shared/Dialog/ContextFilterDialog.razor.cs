using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Sales;
using System;

namespace Sufficit.Blazor.Client.Shared
{
    public partial class ContextFilterDialog : ComponentBase
    {
        [Inject]
        IContextView View { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; internal set; } = default!;

        [CascadingParameter] 
        MudDialogInstance? MudDialog { get; set; }

        protected DialogOptions DialogOptions { get; }
            = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true, FullScreen = false };

        protected string? FilterText { get; set; }
        
        //public TextSearchControl? TextSearch { get; internal set; }
        
        protected override void OnInitialized()
        {
            base.OnInitialized();
            //TextSearch = new TextSearchControl(Navigation);
        }

        public async void OnClientSelect(IClient client)
        {
            await View.Update(client.Id);
        }

        public async void OnClearClicked(MouseEventArgs _)
        {
            await View.Update(Guid.Empty);
            MudDialog?.Close();
        }

        void Close() => MudDialog?.Close();
    }
}
