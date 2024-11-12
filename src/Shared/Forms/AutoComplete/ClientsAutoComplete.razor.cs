using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Shared.Forms.AutoComplete
{
    public partial class ClientsAutoComplete : GenericAutoComplete<IIdTitlePair>
    {
        [Parameter]
        public uint TimeOut { get; set; } = 1500;

        CancellationTokenSource? TokenSource;

        protected async override Task<IEnumerable<IIdTitlePair>> Search(string? filter, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                TokenSource?.Cancel(false);

                TokenSource = new CancellationTokenSource((int)TimeOut);
                return await APIClient.Sales.GetClients(filter, 5, TokenSource.Token);
            }

            return [];
        }
    }
}
