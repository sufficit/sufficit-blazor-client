using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class ContactAvatar
    {
        [Inject]
        public IOptions<ContactsOptions> Options { get; set; } = default!;

        [Parameter]
        [Obsolete("use ReferenceId instead")]
        public Guid IDReference { get => ReferenceId; set => ReferenceId = value; }

        [EditorRequired]
        [Parameter]
        public Guid ReferenceId { get; set; }

        [Parameter]
        public Size Size { get; set; }

        [Parameter]
        public int? MaxDiameter { get; set; }

        private string style => "background-color: white; vertical-align: middle;";

        protected string? Style =>
            MaxDiameter != null ? $"{style} max-width: {MaxDiameter}px; max-height: {MaxDiameter}px;" : style;

        protected string SourceUrl => $"{Options.Value.AvatarPath}?IDContexto={ReferenceId}";
    }
}
