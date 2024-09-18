using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Blazor.Components;
using System;

namespace Sufficit.Blazor.Client.Pages.Dashboard
{
    public partial class Material : BasePageComponent, IPage
    {
        #region INTERFACE IPAGE

        static string IPage.RouteParameter => RouteParameter;
        public const string RouteParameter = "/dashboard/material";

        public const string Title = "Material";
        protected override string Description => "Painel em Material Design";

        #endregion
    }
}