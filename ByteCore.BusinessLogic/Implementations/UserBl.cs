using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;
using ByteCore.Helpers;

namespace ByteCore.BusinessLogic.Implementations
{
    public class UserBl : IUserBl
    {
        private readonly ApplicationDbContext _db;

        public UserBl(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User> RegisterUserAsync(string name, string email, string password)
        {
            if (_db.Users.Any(u => u.Email == email || u.Name == name))
            {
                throw new InvalidOperationException("User with this name or email already exists.");
            }
            
            var hashedPassword = PasswordHasher.HashPassword(password);

            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = hashedPassword,
                EnrolledCourses = new List<Course>()
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public User AuthenticateUser(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return user;
        }

        public User GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<User> UpdateUserAsync(string currentEmail, User updatedUser)
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