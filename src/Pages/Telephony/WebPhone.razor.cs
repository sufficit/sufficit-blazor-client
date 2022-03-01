using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Sufficit.Client;
using Sufficit.Blazor.UI.Material.Extensions;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Pages.Telephony
{
    [Authorize]
    public partial class WebPhone : TelephonyBasePageComponent
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public JsSIPService JsSIPService { get; set; }

        [Inject]
        public ILogger<WebPhone> Logger { get; set; }

        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }

        [Inject]
        IOptions<JsSIPOptions> Options { get; set; }

        [Inject]
        APIClientService APIClient { get; set; }

        protected override string Title => "Telefone Web";

        protected override string Description => "Aplicativo de telefone virtual";


        protected string Destination { get; set; }

        /// <summary>
        /// key used to connect on asterisk server
        /// </summary>
        protected Guid WebRTCKey { get; set; }

        protected IEnumerable<JsSIPMediaDevice> MediaDevices { get; set; } = new JsSIPMediaDevice[] { };

        protected IEnumerable<JsSIPMediaDevice> AudioInputDevices => MediaDevices.Where(s => s.Kind == "audioinput");

        protected IEnumerable<JsSIPMediaDevice> AudioOutputDevices => MediaDevices.Where(s => s.Kind == "audiooutput");

        protected IEnumerable<JsSIPMediaDevice> VideoInputDevices => MediaDevices.Where(s => s.Kind == "videoinput");

        protected override async Task OnInitializedAsync()
        {
            JsSIPService.OnChanged += (sender, args) => StateHasChanged();
            if (string.IsNullOrWhiteSpace(JsSIPService.Status))
            {
                var authstate = await AuthProvider.GetAuthenticationStateAsync();
                var idClaim = authstate.User.FindFirst(s => s.Type == "sub");
                if (Guid.TryParse(idClaim.Value, out Guid UserID))
                {
                    WebRTCKey = await APIClient.Telephony.WebRTCKey();
                    var endpoint = $"{UserID.ToString("N")}{WebRTCKey.ToString("N")}";
                    var options = Options.Value;
                    options.Uri = $"sip:{ endpoint }@voip.sufficit.com.br";
                    await JsSIPService.Start(options);
                }
            }
            MediaDevices = await JsSIPService.MediaDevices();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsSIPService.TestDevices();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task VoiceCall() => await JsSIPService.Call(Destination, false);
        protected async Task VideoCall() => await JsSIPService.Call(Destination, true);

        protected void SetDevice(JsSIPMediaDeviceKind kind, string id)
        {
            JsSIPService.Devices.Update(kind, id);
            Console.WriteLine($"{kind} :: {id}");
        }

    }
}
