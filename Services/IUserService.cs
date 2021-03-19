using SufficitBlazorClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
    }
}
