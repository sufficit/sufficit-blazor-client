using AsterNET.Manager.Event;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Sufficit.AsteriskManager.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Components
{
    public class SignalRQStatusComponent : ComponentBase, IAsyncDisposable
    {
        [Inject]
        protected ISnackbar Snackbar { get; set; }

        [Parameter]
        public string Extension { get; set; }

        protected QueueStatus AMIQueue;
        protected MessagesBuffer<QueueCallerAbandonEventInterface> Buffer = new MessagesBuffer<QueueCallerAbandonEventInterface>(100);
        protected HubConnection hubConnection;
        protected string userInput;
        protected string filterInput;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrWhiteSpace(Extension))
            {
                AMIQueue = new QueueStatus(Extension);
                AMIQueue.UpdatedEvent += StateHasChanged;   

                hubConnection = new HubConnectionBuilder()
                    .WithUrl(new Uri("https://localhost:26507/amiqueuehub"))
                    .Build();

                hubConnection.On<string>("System", SystemCommunication);
                hubConnection.On<string, AMIQueueParamsEvent>("Event:QueueParams", AMIQueue.ReceiveEventHandler);
                hubConnection.On<string, AMIQueueMemberEvent>("Event:QueueMember", AMIQueue.ReceiveEventHandler);
                hubConnection.On<string, AMIQueueMemberStatusEvent>("Event:QueueMemberStatus", AMIQueue.ReceiveEventHandler);
                //hubConnection.On<string, AMIQueueStatusCompleteEvent>("Event:QueueStatusComplete", AMIQueue.ReceiveEventHandler);
                hubConnection.Reconnecting += OnHubReconnecting;
                hubConnection.Reconnected += OnHubReconnected;
                hubConnection.Closed += OnHubClosed;

                try
                {
                    await hubConnection.StartAsync();
                }
                catch (Exception ex) { await OnHubClosed(ex); }

                await hubConnection.SendAsync("RequestQUpdate", Extension);
                await hubConnection.SendAsync("SetFilter", Extension);
            }
        }

        #region CONECTIONS EVENTS

        protected async Task DisposeConnection()
        {
            await hubConnection.DisposeAsync();
        }

        async Task OnHubReconnected(string s)
        {
            SystemCommunication($"reconected: {s}");
            await hubConnection.SendAsync("SetFilter", filterInput);
        }

        async Task OnHubClosed(Exception ex)
        {
            if (ex != null)
            {
                SystemCommunication($"error on closed: {ex.Message}");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (hubConnection.State != HubConnectionState.Connected)
                {
                    try
                    {
                        await hubConnection.StartAsync();
                    }
                    catch (Exception exx)
                    {
                        SystemCommunication($"error on retrying ({sw.ElapsedMilliseconds})ms: {exx.Message}");
                        await Task.Delay(2000);
                    }
                }
                await hubConnection.SendAsync("SetFilter", filterInput);
            }
            await Task.CompletedTask;
        }

        async Task OnHubReconnecting(Exception ex)
        {
            if (ex != null)
            {
                SystemCommunication($"error on reconecting: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        #endregion

        protected async Task OnSetFilter(string e)
        {
            await hubConnection.SendAsync("SetFilter", e);
            filterInput = e;

            // Aguarda um pouco para o filtro ser aplicado corretamente
            await Task.Delay(200);
            Buffer.Clear();
            //AMIQueue.Events = 0;
        }

        void SystemCommunication(string message)
        {
            AddSnackbar(message);
            StateHasChanged();
        }

        public void AddSnackbar(string message)
        {
            Snackbar.Add(message, Severity.Normal, (options) =>
            {
                options.CloseAfterNavigation = true;
            });
        }

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}
