using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Models
{
    public class BlazorServerAuthData
    {
        public string SubjectId { get; set; } = default!;
        public DateTimeOffset Expiration { get; set; }
        public string IdToken { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset RefreshAt { get; set; }
    }
}
