using System;
using System.Text.Json;

namespace Sufficit.Blazor.Client.Services
{
    /// <summary>
    /// Salva informaçãoes na base local (cliente) do WebAssembly
    /// </summary>
    public class WebAssemblyLocalStorageService
    {
        public string Property { get; set; } = "Initial value from StateContainer";

        public event Action OnChange;

        public void SetProperty(string value)
        {
            Property = value;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public int CounterValue { get; set; }

        public string GetStateForLocalStorage()
        {
            return JsonSerializer.Serialize(this);
        }

        public void SetStateFromLocalStorage(string locallyStoredState)
        {
            var deserializedState =
                JsonSerializer.Deserialize<WebAssemblyLocalStorageService>(locallyStoredState);

            CounterValue = deserializedState.CounterValue;
        }
    }
}
