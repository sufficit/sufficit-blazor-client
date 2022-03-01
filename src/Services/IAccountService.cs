using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public interface IAccountService
    {
        Task<bool> LoginAsync();
        Task<bool> LogoutAsync();
    }
}
