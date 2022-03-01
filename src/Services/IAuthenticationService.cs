using SufficitBlazorClient.Models.Identity;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public interface IAuthenticationService
    {
        CustomRemoteUserAccount User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }
}
