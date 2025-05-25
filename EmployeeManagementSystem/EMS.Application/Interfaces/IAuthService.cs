using EMS.Domain.Auth;

namespace EMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
        Task<User?> GetUserByIdAsync(int id);
    }
}
