using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Services
{
    public class CustomAccessTokenProviderAccessor : IAccessTokenProviderAccessor
    {
        public IAccessTokenProvider TokenProvider { get; set; }
    }
}
