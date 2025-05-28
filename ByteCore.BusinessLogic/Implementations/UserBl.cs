using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ByteCore.BusinessLogic.APIs;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;
using ByteCore.Helpers;

namespace ByteCore.BusinessLogic.Implementations
{
    public class UserBl : UserApi, IUserBl
    {
        public async Task<User> RegisterUserAsync(
            string name,
            string email,
            string password,
            string browser = null,
            string ip = null,
            string userAgent = null)
        {
            if (!IsValidEmail(email))
                throw new InvalidOperationException("Invalid email format.");

            var existingByEmail = GetUserByEmailAction(email);
            if (existingByEmail != null)
                throw new InvalidOperationException("User with this email already exists.");

            var existingByName = GetAllAction(1, int.MaxValue)
                                  .Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingByName)
                throw new InvalidOperationException("User with this name already exists.");

            var hashed = PasswordHasher.HashPassword(password);
            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = hashed,
                Role = name.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User",
                RegistrationTime = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                LatestBrowserUsed = browser,
                EnrolledCourses = new List<UserCourse>()
            };
            var loginLog = new LoginLog { User = newUser, IpAddress = ip, UserAgent = userAgent };
            return await RegisterUserAction(newUser, loginLog);
        }

        public User AuthenticateUser(
            string email,
            string password,
            string browser = null,
            string ip = null,
            string userAgent = null)
        {
            if (!IsValidEmail(email))
                throw new InvalidOperationException("Invalid email format.");

            var hashed = PasswordHasher.HashPassword(password);
            var log = new LoginLog { User = new User { Email = email }, IpAddress = ip, UserAgent = userAgent };
            var user = AuthenticateUserAction(email, hashed, log);
            if (user != null && browser != null)
                user.LatestBrowserUsed = browser;
            return user;
        }

        public User GetUserByEmail(string email)
        {
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format.");
            return GetUserByEmailAction(email);
        }

        public async Task<User> UpdateUserAsync(string currentEmail, User updatedUser)
        {
            if (!IsValidEmail(currentEmail) || !IsValidEmail(updatedUser.Email))
                throw new ArgumentException("Invalid email format.");

            var existing = GetUserByEmailAction(currentEmail);
            if (existing == null)
                throw new InvalidOperationException("User not found.");

            var conflictEmail = GetUserByEmailAction(updatedUser.Email);
            if (conflictEmail != null && conflictEmail.Id != existing.Id)
                throw new InvalidOperationException("Email already in use.");

            var allUsers = GetAllAction(1, int.MaxValue);
            if (allUsers.Any(u => u.Name.Equals(updatedUser.Name, StringComparison.OrdinalIgnoreCase)
                                   && u.Id != existing.Id))
            {
                throw new InvalidOperationException("Name already in use.");
            }

            return await UpdateUserAction(existing, updatedUser);
        }

        public async Task<HttpCookie> GetUserCookieAsync(string email, bool rememberMe = false)
        {
            if (!IsValidEmail(email))
                throw new InvalidOperationException("Invalid email format.");
            var user = GetUserByEmailAction(email);
            if (user == null)
                throw new InvalidOperationException("User not found for cookie creation.");
            return await GetUserCookieAction(user, rememberMe);
        }

        public User GetUserByCookie(string cookie)
        {
            var user = GetUserByCookieAction(cookie);
            if (user?.LastSeen < DateTime.UtcNow.AddMinutes(-5))
                user.LastSeen = DateTime.UtcNow;
            return user;
        }

        public int GetUserCount() => GetUserCountAction();
        public User GetFirstUser() => GetFirstUserAction();
        public User GetLastUser() => GetLastUserAction();
        public Dictionary<string, int> GetBrowserUsages() => GetBrowserUsagesAction();
        public List<int> GetActiveUserCount(DateTime fromDate, DateTime toDate) => GetActiveUserCountAction(fromDate, toDate);
        public List<int> GetUserCountByRegistrationDate(DateTime fromDate, DateTime toDate) => GetUserCountByRegistrationDateAction(fromDate, toDate);
        public List<LoginLog> GetLoginLogs(int page = 1, int pageSize = 20) => GetLoginLogsAction(page, pageSize);
        public int GetLoginLogCount() => GetLoginLogCountAction();
        public IEnumerable<User> GetAll(int page = 1, int pageSize = 20) => GetAllAction(page, pageSize);
        public void UpdateUserRange(IEnumerable<User> users) => UpdateUserRangeAction(users);
    }
}
