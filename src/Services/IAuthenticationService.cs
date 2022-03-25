using Sufficit.Blazor.Client.Models.Identity;
using System.Threading.Tasks;

namespace Sufficit.Blazor.Client.Services
{
    public interface IAuthenticationService
    {
        CustomRemoteUserAccount User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }
}
