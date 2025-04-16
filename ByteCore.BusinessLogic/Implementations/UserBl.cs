using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                Role = "User",
                EnrolledCourses = new List<Course>()
            };
            
            if (newUser.Name == "admin")
                newUser.Role = "Admin";

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

        public async Task<HttpCookie> GetUserCookieAsync(string email, bool rememberMe = false)
        {
            var cookie = new HttpCookie("X-KEY")
            {
                Value = CookieGenerator.Create(email),
            };

            var session = await _db.UserSessions.FirstOrDefaultAsync(x => x.User.Email == email);
            var expireTime = rememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(1);
            if (session == null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
                session = new UserSession
                {
                    User = user,
                    CookieString = cookie.Value,
                    ExpireTime = expireTime
                };
                _db.UserSessions.Add(session);
            }
            else
            {
                session.CookieString = cookie.Value;
                if (session.ExpireTime < expireTime)
                    session.ExpireTime = expireTime;
            }
            
            await _db.SaveChangesAsync();
            cookie.Expires = session.ExpireTime;
            return cookie;
        }

        public User GetUserByCookie(string cookie)
        {
            var session = _db.UserSessions
                .Include(x => x.User)
                .FirstOrDefault(x => x.CookieString == cookie && x.ExpireTime >= DateTime.UtcNow);

            return session?.User;
        }
    }
}