using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Models
{
    public class BreadcrumbBuilder
    {
        public string Title { get; set; } = default!;
        public Action? Action { get; set; }
        public string? Url { get; set; }
        public IEnumerable<KeyValuePair<string, object>>? Attributes { get; set; }


        public string? LeftIcon { get; set; }
        public Action? LeftAction { get; set; }
        public IEnumerable<KeyValuePair<string, object>>? LeftAttributes { get; set; }

        public string? RightIcon { get; set; }
        public Action? RightAction { get; set; }
        public IEnumerable<KeyValuePair<string, object>>? RightAttributes { get; set; }
    }
}
