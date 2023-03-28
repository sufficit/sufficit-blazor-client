using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Sufficit.Client;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sufficit.Identity;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    [Authorize]
    public partial class WebPhone : TelephonyBasePageComponent
    {
        [Inject]
        public JsSIPService JsSIPService { get; set; } = default!;

        [Inject]
        public ILogger<WebPhone> Logger { get; set; } = default!;

        [Inject]
        IAuthenticationStateProvider AuthenticationService { get; set; } = default!;

        [Inject]
        IOptions<JsSIPOptions> Options { get; set; } = default!;

        [Inject]
        APIClientService APIClient { get; set; } = default!;

        protected override string Title => "Telefone Web";

        protected override string Description => "Aplicativo de telefone virtual";

        protected string? Destination { get; set; }

        /// <summary>
        /// key used to connect on asterisk server
        /// </summary>
        protected Guid WebRTCKey { get; set; }

        protected IEnumerable<JsSIPMediaDevice> MediaDevices { get; set; } = new JsSIPMediaDevice[] { };

        protected IEnumerable<JsSIPMediaDevice> AudioInputDevices => MediaDevices.Where(s => s.Kind == "audioinput");

        protected IEnumerable<JsSIPMediaDevice> AudioOutputDevices => MediaDevices.Where(s => s.Kind == "audiooutput");

        protected IEnumerable<JsSIPMediaDevice> VideoInputDevices => MediaDevices.Where(s => s.Kind == "videoinput");


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var responseTest = await JsSIPService.TestDevices(new Sufficit.Telephony.JsSIP.Methods.TestDevicesRequest() { Audio = true, Video = true });
                Logger.LogWarning("testing devices status: {success}, message: {message}", responseTest.Success, responseTest.Message);

                JsSIPService.OnChanged += (sender, args) => StateHasChanged();
                if (string.IsNullOrWhiteSpace(JsSIPService.Status))
                {
                    var authstate = await AuthenticationService.GetAuthenticationStateAsync();
                    var UserID = authstate.User.GetUserId();
                    if (UserID != Guid.Empty)
                    {
                        WebRTCKey = await APIClient.Telephony.WebRTCKey();
                        var endpoint = $"{UserID.ToString("N")}{WebRTCKey.ToString("N")}";
                        var options = Options.Value;
                        options.Uri = $"sip:{endpoint}@voip.sufficit.com.br";
                        await JsSIPService.Start(options);
                    }
                }

                MediaDevices = await JsSIPService.MediaDevices();
            }
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
