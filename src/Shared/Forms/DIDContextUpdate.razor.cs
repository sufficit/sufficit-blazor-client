using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using Sufficit.Client;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms
{
    public partial class DIDContextUpdate : ComponentBase
    {
        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [EditorRequired]
        [Parameter]
        public Sufficit.Telephony.DirectInwardDialingV1? Item { get; set; } = default!;

        protected override void OnParametersSet()
        {
            if (Item != null)
                newContextId = Item.ContextId;             
        }

        protected bool UpdatePending
            => Item != null && Item.ContextId != newContextId;

        protected Guid newContextId;

        protected void OnValueChanged(IIdTitlePair? pair)
        {
            if (Item == null) return;
            if (pair == null || pair.Id == Guid.Empty) return;
                        
            newContextId = pair.Id;            
        }
    }
}
