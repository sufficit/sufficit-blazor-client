﻿using Microsoft.AspNetCore.Components;
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
            await InvokeAsync(StateHasChanged);           
        }        

        protected void OnValedictionChange(Guid selected)
        {
            Item.IdValediction = selected;
        }

        protected void OnAnnouncementChange(Guid selected)
        {
            Item.IdAnnouncement = selected;
        }  
    }
}
