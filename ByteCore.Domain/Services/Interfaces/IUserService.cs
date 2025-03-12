using System.Threading.Tasks;
using ByteCore.Domain.Entities;
using ByteCore.Model.Models;

namespace ByteCore.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(string name, string email, string password);
        User AuthenticateUser(string email, string password);
        User GetUserByEmail(string email);
        Task<User> UpdateUserAsync(string currentEmail, User updatedUser);
    }
}