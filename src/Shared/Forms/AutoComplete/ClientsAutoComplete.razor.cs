using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sufficit.Contacts;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms.AutoComplete
{
    public partial class ClientsAutoComplete : GenericAutoComplete<IIdTitlePair>
    {      
        protected async override Task<IEnumerable<IIdTitlePair>> Search(string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                return await APIClient.Sales.GetClients(filter, 5, default);
            }

            return Array.Empty<IIdTitlePair>();
        }
    }
}
