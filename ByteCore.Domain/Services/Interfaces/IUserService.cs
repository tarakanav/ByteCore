using System.Threading.Tasks;
using ByteCore.Model.Models;

namespace ByteCore.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(string name, string email, string password);
        UserModel AuthenticateUser(string email, string password);
        UserModel GetUserByEmail(string email);
        Task<UserModel> UpdateUserAsync(string currentEmail, UserModel updatedUser);
    }
}