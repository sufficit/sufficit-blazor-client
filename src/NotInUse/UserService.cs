using SufficitBlazorClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class UserService : IUserService
    {
        private IHttpService _httpService;

        public UserService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<IEnumerable<CustomRemoteUserAccount>> GetAll()
        {
            return await _httpService.Get<IEnumerable<CustomRemoteUserAccount>>("/users");
        }
    }
}
