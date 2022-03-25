using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Services
{
    public class TestAuthenticationService : AuthenticationService
    {
        public TestAuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform) : base(schemes, handlers, transform)
        {

        }
    }
}
