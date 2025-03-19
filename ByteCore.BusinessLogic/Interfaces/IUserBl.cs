using System.Threading.Tasks;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface IUserBl
    {
        Task<User> RegisterUserAsync(string name, string email, string password);
        User AuthenticateUser(string email, string password);
        User GetUserByEmail(string email);
        Task<User> UpdateUserAsync(string currentEmail, User updatedUser);
    }
}