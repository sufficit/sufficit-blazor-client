using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components
{
    public partial class ContactAvatar
    {
        [Parameter]
        [Obsolete("use ReferenceId instead")]
        public Guid IDReference { get => ReferenceId; set => ReferenceId = value; }

        [Parameter]
        public Guid ReferenceId { get; set; }

        [Parameter]
        public Size Size { get; set; }

        [Parameter]
        public int? MaxDiameter { get; set; }

        protected string? Style =>
            MaxDiameter != null ? $"max-width: {MaxDiameter}px; max-height: {MaxDiameter}px;" : null;

        protected string SourceUrl => $"https://www.sufficit.com.br/Relacionamento/Avatar.ashx?IDContexto={ReferenceId}";
    }
}
