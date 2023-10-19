using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Sufficit.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Components.Contacts
{
    public partial class ContactAvatar : ComponentBase
    {
        [Inject]
        public IOptions<ContactsOptions> Options { get; set; } = default!;

        [Parameter]
        [Obsolete("use ReferenceId instead")]
        public Guid IDReference { get => ReferenceId; set => ReferenceId = value; }

        [EditorRequired]
        [Parameter]
        public Guid ReferenceId { get; set; }

        /// <summary>
        ///     If specified, sets the exact size for this avatar <br />
        ///     Otherwise, fluid content
        /// </summary>
        [Parameter]
        public Size? Size { get; set; }

        [Parameter]
        public int? MaxDiameter { get; set; }

        /// <summary>
        ///     Box shadow => elevation effect level
        /// </summary>
        [Parameter]
        public int? Elevation { get; set; }

        /// <summary>
        ///     MudAvatar element reference
        /// </summary>
        protected MudAvatar MudAvatarElement { get; set; } = default!;

        private IDictionary<string, string> staticstyles { get; }

        public ContactAvatar()
        {
            staticstyles = new Dictionary<string, string>();
            staticstyles["background-color"] = "white";
            staticstyles["vertical-align"] = "middle";
        }

        protected string Style 
        { 
            get {
                var dic = new Dictionary<string, string>();
                
                if (MaxDiameter != null) {
                    dic["max-width"] = $"{MaxDiameter}px";
                    dic["max-height"] = $"{MaxDiameter}px";
                }

                if (Size == null)
                {
                    dic["width"] = "unset";
                    dic["height"] = "unset";
                    dic["font-size"] = "unset";
                }

                var styles = dic.Union(staticstyles).Select(s => $"{s.Key}: {s.Value}");
                return string.Join(';', styles);
            } 
        }

        protected string SourceUrl => $"{Options.Value.AvatarPath}?contextid={ReferenceId}";
    }
}
