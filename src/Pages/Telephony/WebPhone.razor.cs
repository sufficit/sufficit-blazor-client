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
using Sufficit.Telephony;

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

        protected JsSIPSessionMonitor? CallSession { get; set; }


        private bool IsCalling = false;
        private string PhoneNumber = string.Empty;
        protected void SetIsCalling(string Number)
        {
            IsCalling = !IsCalling;
            PhoneNumber = Number;
        }

        protected override string Title => "Telefone Web";

        protected override string Description => "Aplicativo de telefone virtual";

        /// <summary>
        /// key used to connect on asterisk server
        /// </summary>
        protected Guid WebRTCKey { get; set; }

        protected IEnumerable<JsSIPMediaDevice> MediaDevices { get; set; } = Array.Empty<JsSIPMediaDevice>();

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
                        var endpoint = $"{UserID:N}{WebRTCKey:N}";
                        var options = Options.Value;
                        options.Uri = $"sip:{endpoint}@voip.sufficit.com.br";
                        await JsSIPService.Start(options);
                    }
                }

                MediaDevices = await JsSIPService.MediaDevices();
            }
        }

        protected async Task CallStart(string? destination)
        {
            if (!string.IsNullOrWhiteSpace(destination))
            {
                CallSession = await JsSIPService.CallMonitor(destination, false);
                SetIsCalling(destination);
            }
        }

        protected async Task VoiceCall(string Destination) => await JsSIPService.Call(Destination, false);
        protected async Task VideoCall(string Destination) => await JsSIPService.Call(Destination, true);

        protected void SetDevice(JsSIPMediaDeviceKind kind, string id)
        {
            JsSIPService.Devices.Update(kind, id);
            Console.WriteLine($"{kind} :: {id}");
        }

    }
}
