using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.Web.Data;
using ByteCore.Web.Models;

namespace ByteCore.Web.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordService _passwordService;

        public UserService(ApplicationDbContext db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        public async Task<UserModel> RegisterUserAsync(string name, string email, string password)
        {
            if (_db.Users.Any(u => u.Email == email || u.Name == name))
            {
                throw new InvalidOperationException("User with this name or email already exists.");
            }
            
            var hashedPassword = _passwordService.HashPassword(password);

            var newUser = new UserModel
            {
                Name = name,
                Email = email,
                Password = hashedPassword,
                EnrolledCourses = new List<CourseModel>()
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public UserModel AuthenticateUser(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !_passwordService.VerifyPassword(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return user;
        }

        public UserModel GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<UserModel> UpdateUserAsync(string currentEmail, UserModel updatedUser)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == currentEmail);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            
            if (_db.Users.Any(u => u.Name == updatedUser.Name && u.Email != currentEmail))
            {
                throw new InvalidOperationException("User with this name already exists.");
            }
            
            if (_db.Users.Any(u => u.Email == updatedUser.Email && u.Email != currentEmail))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
            
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            await _db.SaveChangesAsync();
            
            return user;
        }
    }
}