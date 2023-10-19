using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Telephony.JsSIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sufficit.Identity;
using Sufficit.Telephony.JsSIP.Methods;
using System.Text.Json;
using Sufficit.Telephony;
using System.Threading;

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
        IOptions<JsSIPOptions> Options { get; set; } = default!;

        [Inject]
        APIClientService APIClient { get; set; } = default!;

        [CascadingParameter]
        public UserPrincipal User { get; set; } = default!;

        protected string? JsSIPVersion { get; set; }

        protected TestDevicesResponse? Testing { get; set; }

        protected JsSIPSessionMonitor? CallSession { get; set; }

        protected bool CanOriginate => JsSIPService.IsConnected;

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
            if (!firstRender) return;
            
            JsSIPVersion = await JsSIPService.GetVersion();

            var testRequest = new Sufficit.Telephony.JsSIP.Methods.TestDevicesRequest() { Audio = true, Video = false };
            Testing = await JsSIPService.TestDevices(testRequest);

            JsSIPService.OnChanged += ServiceChanged;
            JsSIPService.Monitor.OnChanged += OnMonitorChanged;
            if (string.IsNullOrWhiteSpace(JsSIPService.Status))
            {
                var userid = User.GetUserId();
                if (userid != Guid.Empty)
                {
                    WebRTCKey = await APIClient.Telephony.WebRTC.GetKey(CancellationToken.None) ?? Guid.Empty;
                    var endpoint = $"{userid:N}{WebRTCKey:N}";
                    var options = Options.Value;
                    options.Uri = $"sip:{endpoint}@voip.sufficit.com.br";
                    options.Password = WebRTCKey.ToString("N");

                    var json = JsonSerializer.Serialize(options);
                    Logger.LogWarning("options: {0}", json);
                    await JsSIPService.Start(options);
                }
            }

            MediaDevices = await JsSIPService.GetMediaDevices();
            await JsSIPService.RequestMediaAccess();
        }

        protected async void ServiceChanged(object? sender, EventArgs args)
            => await InvokeAsync(StateHasChanged);

        protected async void OnMonitorChanged(object? sender, string? id)
        {
            if (!string.IsNullOrWhiteSpace(id)) 
            {
                var info = await JsSIPService.Sessions.GetSession(id);
                CallSession = JsSIPService.Sessions.CallMonitor(info);
                CallSession.OnChanged += OnCallSessionChanged;
                CallSession.OnAcknowledge += OnCallSessionAcknowledge;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task CallStart(string? destination)
        {
            if (!string.IsNullOrWhiteSpace(destination))
            {
                CallSession = await JsSIPService.CallMonitor(destination, false);
                CallSession.OnChanged += OnCallSessionChanged;
                CallSession.OnAcknowledge += OnCallSessionAcknowledge;
                SetIsCalling(destination);
            }
        }

        private async void OnCallSessionAcknowledge(object? sender, EventArgs e)
        {
            if (CallSession != null)
            {
                CallSession.OnAcknowledge -= OnCallSessionAcknowledge;
                CallSession = null;
            }
            await InvokeAsync(StateHasChanged);
        }

        protected async void OnCallSessionChanged(object? sender, EventArgs args)
        {
            // auto close if success terminated the call, no errors
            if (sender is JsSIPSession session)
            {
                if (session.Cause == JsSIPSessionCause.BYE)
                {
                    session.OnChanged -= OnCallSessionChanged;
                   
                    // waiting a while before refresh
                    await Task.Delay(2500); 

                    CallSession = null;               
                }
            }

            await InvokeAsync(StateHasChanged);
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