﻿using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.Client.Models;
using System;
using System.Collections.Generic;

namespace Sufficit.Blazor.Client.Pages
{
    public class BasePageComponent : ComponentBase, IDisposable
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
   
        [Inject]
        protected IBreadcrumbService BreadcrumbService { get; set; }

        protected virtual string Title { get; }

        protected virtual string Description { get; }

        protected virtual string Keywords { get; }

        protected override void OnParametersSet()
        {
            // continuando o processo na base do componente
            base.OnParametersSet();

            BreadcrumbService
            .Set<HomeBreadcrumb>()
            .Set<PagesBreadcrumb>()
            .Set<PageBreadcrumb>(new Dictionary<string, object>
            {
                ["title"] = Title,
                ["description"] = Description,
                ["link"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
            });

            BreadcrumbService.Description = Description;
        }

        void IDisposable.Dispose()
        {
            // Limpando as informações no menu de navegação
            BreadcrumbService.Clear();
        }
    }
}
