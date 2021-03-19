using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SufficitBlazorClient.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SufficitBlazorClient
{
    public class ApplicationAuthenticationState : RemoteAuthenticationState
    {
        public event Action OnChange;

        /// <summary>
        /// Aciona os eventos de mudança caso existam
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();

        public string session_state { get; internal set; }

        private string _id;
        public string Id { get { return _id; } set { _id = value; NotifyStateChanged(); } }


        public string GetStateForLocalStorage()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Recupera o elemento do armazenamento do cliente e atualiza as informações
        /// </summary>
        /// <param name="locallyStoredState"></param>
        public void SetStateFromLocalStorage(string locallyStoredState)
        {
            if (!string.IsNullOrWhiteSpace(locallyStoredState))
            {
                var deserializedState =
                    JsonSerializer.Deserialize<ApplicationAuthenticationState>(locallyStoredState);

                SetStateFromLocalStorage(deserializedState);
            }
        }

        public void SetStateFromLocalStorage(ApplicationAuthenticationState deserializedState)
        {
            if (deserializedState != null)
            {
                bool shouldUpdate = false;

                if (_id != deserializedState.Id)
                {
                    _id = deserializedState.Id;
                    shouldUpdate = true;
                }

                this.ReturnUrl = deserializedState.ReturnUrl;

                session_state = deserializedState.session_state;

                // Aciona os eventos de atualização
                if (shouldUpdate) NotifyStateChanged();
            }
        }
    }
}
