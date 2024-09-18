using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Sufficit.Sales.Constants;

namespace Sufficit.Blazor.Client.Pages.Sales
{
    [Authorize(Roles = SalesManagerRole.NormalizedName)]
    public partial class Contracts : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/sales/contracts";

        public const string? Icon = Icons.Material.Filled.EventRepeat;
        public const string Title = "Serviços recorrentes";
        protected override string? Area => "Vendas";

        #endregion

        //Style="@($"color:{Colors.LightGreen.Accent3}; background:{Colors.BlueGrey.Darken4};")"
        protected string ExpiredStyle = $"background:{Colors.Red.Lighten4}";

        #region VISUAL ADJUSTS

        protected string GetExpired(DateTime? end)
        {
            return (end < DateTime.Now) ? ExpiredStyle : string.Empty;
        }

        protected string? KeyFormat(string? key)
        {
            if (Sufficit.Telephony.Utils.IsValidPhoneNumber(key, false))
                return Sufficit.Telephony.Utils.FormatarTelefoneE164Semantic(key!);
            else
                return key;
        }

        protected string GetIcon(string title)
        {
            switch (title)
            {
                case SERVICE_TRUNK_INBOUND: return @Icons.Material.Filled.TripOrigin;
                case SERVICE_FLASH_OPERATOR_PANEL: return @Icons.Material.Filled.SmartButton;
                case SERVICE_BUSINESS_SUPPORT: return @Icons.Material.Filled.SupportAgent;
                case SERVICE_TRUNK_FIXED_UNLIMITED:
                case SERVICE_TRUNK_MOBILE_UNLIMITED:
                    return @Icons.Material.Filled.Start;
                default: return @Icons.Material.Filled.Pending;
            }
        }

        #endregion

        [Inject]
        APIClientService Service { get; set; } = default!;

        [Inject]
        IContextView ContextView { get; set; } = default!;

        protected bool ShowExpired { get; set; }

        protected IEnumerable<Contract> Items = Array.Empty<Contract>();

        private string? Error;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

            ContextView.OnChanged += ContextChanged;

            var contextid = ContextView.ContextId.GetValueOrDefault();
            if (contextid != Guid.Empty)
                await Filter(contextid);
        }

        protected async void ContextChanged(Guid? value)
            => await Filter(value.GetValueOrDefault());

        protected async void ShowExpiredChanged(bool value)
        {
            ShowExpired = value;
            await Filter(ContextView.ContextId.GetValueOrDefault());
        }

        protected async Task Filter(Guid ContextId)
        {
            Items = Array.Empty<Contract>();
            Error = string.Empty;

            if (ContextId != Guid.Empty)
            {
                var parameters = new ContractSearchParameters();
                parameters.ContextId = ContextId;
                if (!ShowExpired)
                    parameters.Expiration = new DateTimeRange() { Start = DateTime.Today, Inclusive = true };

                try
                {
                    Items = await Service.Sales.GetContracts(parameters, CancellationToken.None);
                }
                catch (UnauthorizedAccessException)
                {
                    Error = "Não autorizado";
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }

            // refresh front end
            await InvokeAsync(StateHasChanged);
        }
    }
}