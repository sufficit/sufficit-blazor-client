using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material.Components;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony.IVR
{
    public partial class Properties : ComponentBase
    {
        [Parameter]
        [EditorRequired]
        public Sufficit.Telephony.IVR Item { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        protected IEnumerable<Audio>? Audios { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            // Go get sounds
            Audios = await APIClient.Telephony.Audio.ByContext(Item.IdContext);
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
        
        protected void OnValedictionChange(SelectedChangedEventArgs<string?> args)
        {
            if(Guid.TryParse(args.Current, out Guid id))
                Item.IdValediction = id;
        }

        protected void OnAnnouncementChange(SelectedChangedEventArgs<string?> args)
        {
            if (Guid.TryParse(args.Current, out Guid id))
                Item.IdAnnouncement = id;
        }   
        
    }
}
