using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Telephony;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Telephony
{
    [Authorize]
    public partial class Preferences : TelephonyBasePageComponent, IDisposable, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/telephony/preferences";

        public const string? Icon = Icons.Material.Filled.SettingsSuggest;
        public const string Title = "Preferências";
        protected override string Description => "Facilidades para o sistema de telefonia";

        #endregion

        [Inject]
        protected IContextView ContextView { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        protected uint? AreaCodePreference { get; set; }

        protected EndPointProperty? AreaCodeProperty { get; set; }

        protected EndPointProperty? IDForwardProperty { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            ContextView.OnChanged -= OnContextViewChanged;
            ContextView.OnChanged += OnContextViewChanged;

            await LoadPreferences();
        }
                
        protected async Task LoadPreferences()
        {            
            {
                var request = new EndPointPropertyRequest() { Key = "areacode" };
                request.ContextId = ContextView.ContextId.GetValueOrDefault();
                request.EndPointId = Guid.Empty;

                AreaCodePreference = null;
                AreaCodeProperty = await APIClient.Telephony.EndPoint.GetEndPointProperty(request, CancellationToken.None);
                if (AreaCodeProperty != null && AreaCodeProperty.Value != null)
                    AreaCodePreference = uint.Parse(AreaCodeProperty.Value);
            }
            {
                var request = new EndPointPropertyRequest() { Key = "idforward" };
                request.ContextId = ContextView.ContextId.GetValueOrDefault();
                request.EndPointId = Guid.Empty;

                IDForwardProperty = await APIClient.Telephony.EndPoint.GetEndPointProperty(request, CancellationToken.None);
            }            
        }

        protected async Task OnAreaCodeChanged(uint? value)
        {
            AreaCodePreference = value;

            if (AreaCodeProperty == null)
            {
                AreaCodeProperty = new EndPointProperty()
                {
                    Key = "areacode",
                    ContextId = ContextView.ContextId.GetValueOrDefault(),
                    EndPointId = Guid.Empty
                };
            }
            
            AreaCodeProperty.Value = value?.ToString();
            await APIClient.Telephony.EndPoint.PostEndPointProperty(AreaCodeProperty, CancellationToken.None);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task OnIdForwardChanged(string? value)
        {
            if (IDForwardProperty == null)
            {
                IDForwardProperty = new EndPointProperty()
                {
                    Key = "idforward",
                    ContextId = ContextView.ContextId.GetValueOrDefault(),
                    EndPointId = Guid.Empty
                };
            }

            IDForwardProperty.Value = value;
            await APIClient.Telephony.EndPoint.PostEndPointProperty(IDForwardProperty, CancellationToken.None);
            await InvokeAsync(StateHasChanged);
        }

        private async void OnContextViewChanged(Guid? obj)
        {
            await LoadPreferences();
            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            ContextView.OnChanged -= OnContextViewChanged;
        }
    }
}
