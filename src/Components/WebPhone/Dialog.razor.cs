using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Telephony.JsSIP;
using System;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.WebPhone
{
    public partial class Dialog : ComponentBase
    {
        [Parameter]
        public string? PhoneNumber { get; set; }

        [EditorRequired]
        [Parameter]
        public JsSIPSessionMonitor CallSession { get; set; } = default!;

        [Parameter]
        public EventCallback<string> SetIsCalling { get; set; }

        /// <summary>
        ///     Is valid on going session
        /// </summary>
        public bool IsValid 
            => CallSession != null && CallSession.Status > JsSIPSessionStatus.STATUS_NULL;

        public JsSIPSessionEvent? Event
            => CallSession?.Event;

        public string Status 
            => CallSession?.Status switch
            {
                JsSIPSessionStatus.STATUS_NULL => "Inicializando",
                JsSIPSessionStatus.STATUS_INVITE_SENT => "Convite Enviado",
                JsSIPSessionStatus.STATUS_1XX_RECEIVED => "Processando Chamada",
                JsSIPSessionStatus.STATUS_INVITE_RECEIVED => "Convite Recebido",
                JsSIPSessionStatus.STATUS_WAITING_FOR_ANSWER => "Aguardando Atendimento",
                JsSIPSessionStatus.STATUS_ANSWERED => "Atendido",
                JsSIPSessionStatus.STATUS_WAITING_FOR_ACK => "Aguardando Reconhecimento",
                JsSIPSessionStatus.STATUS_CANCELED => "Cancelado",
                JsSIPSessionStatus.STATUS_TERMINATED => "Finalizado",
                JsSIPSessionStatus.STATUS_CONFIRMED => "Atendido",
                _ => "* Desconhecido *"
            };
        

        protected override void OnAfterRender (bool firstRender)
        {
            if (!firstRender) 
                return;

            CallSession.OnChanged += OnCallSessionChanged;        
        }

        protected async void OnCallSessionChanged(object? sender, EventArgs args)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
