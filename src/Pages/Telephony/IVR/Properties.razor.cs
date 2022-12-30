using Microsoft.AspNetCore.Components;
using MudBlazor;
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

        
        protected void OnValedictionChange(Guid selected)
        {
            Item.IdValediction = selected;
        }

        protected void OnAnnouncementChange(Guid? selected)
        {
            Item.IdAnnouncement = selected;
        }   
        

        private async Task<IEnumerable<string>> onSearch(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);


            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
            {
                return (IEnumerable<string>)Audios
                    .Select(e => e.Title)
                    .ToArray();
            }
            else
            {
                return (IEnumerable<string>)Audios
                    .Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                    .Select(e => e.Title)
                    .ToArray();
            }

        }
    }
}
