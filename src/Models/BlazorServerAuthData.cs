using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Models
{
    public class BlazorServerAuthData
    {
        public string SubjectId;
        public DateTimeOffset Expiration;
        public string IdToken;
        public string AccessToken;
        public string RefreshToken;
        public DateTimeOffset RefreshAt;
    }
}
