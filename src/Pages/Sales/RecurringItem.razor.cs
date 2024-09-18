using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Blazor.Components;
using Sufficit.Client;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Pages.Sales
{
    [Authorize(Roles = SalesManagerRole.NormalizedName)]
    public partial class RecurringItem : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/pages/sales/recurringitem";

        public const string? Icon = @Icons.Material.Filled.EventRepeat;
        public const string Title = "Serviço recorrente";
        protected override string? Area => "Vendas";

        #endregion

        [Inject]
        APIClientService Service { get; set; } = default!;

        [Inject]
        IContextView View { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender) return;

        }

        [Inject] ISnackbar Snackbar { get; set; } = default!;

        MudForm form { get; set; } = default!;

        OrderModelFluentValidator orderValidator = new OrderModelFluentValidator();

        OrderDetailsModelFluentValidator orderDetailsValidator = new OrderDetailsModelFluentValidator();

        OrderModel model = new OrderModel();

        public class OrderModel
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string CCNumber { get; set; } = "4012 8888 8888 1881";
            public AddressModel Address { get; set; } = new AddressModel();
            public List<OrderDetailsModel> OrderDetails = new List<OrderDetailsModel>()
        {
            new OrderDetailsModel()
                {
                    Description = "Perform Work order 1",
                    Offer = 100
                },
            new OrderDetailsModel()
        };
        }

        public class AddressModel
        {
            public string Address { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        public class OrderDetailsModel
        {
            public string Description { get; set; } = string.Empty;
            public decimal Offer { get; set; }
        }

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                Snackbar.Add("Submited!");
            }
        }

        /// <summary>
        /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
        /// </summary>
        /// <typeparam name="OrderModel"></typeparam>
        public class OrderModelFluentValidator : AbstractValidator<OrderModel>
        {
            public OrderModelFluentValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress()
                    .MustAsync(async (value, cancellationToken) => await IsUniqueAsync(value));

                RuleFor(x => x.CCNumber)
                    .NotEmpty()
                    .Length(1, 100)
                    .CreditCard();

                RuleFor(x => x.Address.Address)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Address.City)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Address.Country)
                    .NotEmpty()
                    .Length(1, 100);

                RuleForEach(x => x.OrderDetails)
                    .SetValidator(new OrderDetailsModelFluentValidator());
            }

            private async Task<bool> IsUniqueAsync(string email)
            {
                // Simulates a long running http call
                await Task.Delay(2000);
                return email.ToLower() != "test@test.com";
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<OrderModel>.CreateWithOptions((OrderModel)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }

        /// <summary>
        /// A standard AbstractValidator for the Collection Object
        /// </summary>
        /// <typeparam name="OrderDetailsModel"></typeparam>
        public class OrderDetailsModelFluentValidator : AbstractValidator<OrderDetailsModel>
        {
            public OrderDetailsModelFluentValidator()
            {
                RuleFor(x => x.Description)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Offer)
                    .GreaterThan(0)
                    .LessThan(999);
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<OrderDetailsModel>.CreateWithOptions((OrderDetailsModel)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}