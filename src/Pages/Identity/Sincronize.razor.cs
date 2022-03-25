using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;
using Sufficit.Identity.Client;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Sufficit.Blazor.Client.Services;
using System.Linq;
using Sufficit.Blazor.UI.Material.Components;
using System.Threading;
using Microsoft.AspNetCore.Components.Web;

namespace Sufficit.Blazor.Client.Pages.Identity
{
    [Authorize(Roles = "administrator")]
    public partial class Sincronize : BasePageComponent
    {
        protected override string Title => "Sincronia de contas";

        protected override string Description => "Realiza a sincronização dos usuários";

        [Inject]
        IdentityClientService Identity { get; set; }

        [Inject]
        BlazorIdentityService BlazorIdentity { get; set; }

        [Inject]
        ILogger<Sincronize> Logger { get; set; }

        GetUsersResponse Response { get; set; }

        Dictionary<Guid, bool> Checks = new Dictionary<Guid, bool>();

        protected bool Checked(Guid id) => Checks.ContainsKey(id) ? Checks[id] : false;

        protected async Task Start(CancellationToken cancellationToken)
        {
            Response = await Identity.Users.GetUsersAsync(string.Empty, 1, 2000, cancellationToken);
            if (Response != null)
            {
                Checks.Clear();

                int simultaneous = 20;
                int count = Response.Users.Count();
                for (int x = 0; x <= count; x += simultaneous)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    await GroupProcessing(x, x + simultaneous, cancellationToken);
                }
                StateHasChanged();
            }
        }

        protected async Task GroupProcessing(int start, int end, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"processing from: { start } to { end }");
            var tasks = new List<Task>();
            foreach (var user in Response.Users.Take(new Range(start, end)))
            {
                if (cancellationToken.IsCancellationRequested) break;
                var taskSingle = BlazorIdentity.SincronizeUserPolicies(user)
                    .ContinueWith((previous) =>
                    {
                        if (previous.IsCompletedSuccessfully)
                        {
                            Checks[user.ID] = true;
                            StateHasChanged();
                        }
                    });
                tasks.Add(taskSingle);
            }

            await Task.WhenAll(tasks);
        }

        protected CancellationTokenSource Cancellation { get; set; }

        protected async Task ToggleButtonClicked(MouseEventArgs e) 
        {
            if (Cancellation == null)
            {
                Cancellation = new CancellationTokenSource();
                await Start(Cancellation.Token);
            } 
            else
            {
                Cancellation.Cancel();
                Cancellation = null;
            }
        }
    }
}
