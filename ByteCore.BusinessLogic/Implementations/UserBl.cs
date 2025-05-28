using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            {
                throw new InvalidOperationException("Invalid email format.");
            }

            if (UserExistsByEmailOrNameAction(email, name))
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
                EnrolledCourses = new List<UserCourse>(),
                RegistrationTime = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                LatestBrowserUsed = browser,
                LoginLogs = new List<LoginLog>()
            };

            if (newUser.Name == "admin")
                newUser.Role = "Admin";

            var loginLog = CreateLoginLog(newUser, ip, userAgent);
            newUser.LoginLogs.Add(loginLog);

            await AddUserAction(newUser);

            return newUser;
        }

        public User AuthenticateUser(
            string email,
            string password,
            string browser = null,
            string ip = null,
            string userAgent = null)
        {
            return AuthenticateUserAction(email, password, browser, ip, userAgent);
        }
        
        private LoginLog CreateLoginLog(User user, string ip, string userAgent)
        {
            return new LoginLog
            {
                User = user,
                IpAddress = ip,
                UserAgent = userAgent,
                LoginTime = DateTime.UtcNow
            };
        }

        public User GetUserByEmail(string email)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            return GetUserByEmailAction(email);
        }

        public async Task<User> UpdateUserAsync(string currentEmail, User updatedUser)
        {
            if (!IsValidEmail(currentEmail))
            {
                throw new ArgumentException("Invalid current email format.");
            }

            if (!IsValidEmail(updatedUser.Email))
            {
                throw new ArgumentException("Invalid email format for updated user.");
            }

            var user = GetUserByEmailSimpleAction(currentEmail);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            
            if (!user.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) && UserExistsByEmailExcludingIdAction(updatedUser.Email, user.Id))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
            
            if (UserExistsByNameExcludingIdAction(updatedUser.Name, user.Id))
            {
                throw new InvalidOperationException("User with this name already exists.");
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            await SaveUserAction(user);

            return user;
        }

        public async Task<HttpCookie> GetUserCookieAsync(string email, bool rememberMe = false)
        {
            if (!IsValidEmail(email))
            {
                throw new InvalidOperationException("Invalid email format.");
            }

            var cookie = new HttpCookie("X-KEY")
            {
                Value = CookieGenerator.Create(email),
            };

            var session = await GetUserSessionAction(email);
            var expireTime = rememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(1);
            if (session == null)
            {
                var user = GetUserByEmailSimpleAction(email);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found for cookie creation.");
                }
                session = new UserSession
                {
                    User = user,
                    CookieString = cookie.Value,
                    ExpireTime = expireTime
                };
                await AddUserSessionAction(session);
            }
            else
            {
                session.CookieString = cookie.Value;
                if (session.ExpireTime < expireTime)
                    session.ExpireTime = expireTime;
                await UpdateUserSessionAction(session);
            }

            cookie.Expires = session.ExpireTime;
            return cookie;
        }

        public User GetUserByCookie(string cookie)
        {
            return GetUserByCookieAction(cookie);
        }

        public int GetUserCount()
        {
            return GetUserCountAction();
        }

        public User GetFirstUser()
        {
            return GetFirstUserAction();
        }

        public User GetLastUser()
        {
            return GetLastUserAction();
        }

        public Dictionary<string, int> GetBrowserUsages()
        {
            return GetBrowserUsagesAction();
        }

        public List<int> GetActiveUserCount(DateTime fromDate, DateTime toDate)
        {
            var dates = new List<DateTime>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            var users = GetActiveUsersByDateRangeAction(fromDate, toDate);

            var userCountByDate = new List<int>(dates.Count);
            foreach (var date in dates)
            {
                userCountByDate.Add(users.Count(x => x.LastSeenHistory.Select(xx => xx.SeenOn).Contains(date)));
            }

            return userCountByDate;
        }

        public List<int> GetUserCountByRegistrationDate(DateTime fromDate, DateTime toDate)
        {
            var startDate = fromDate.Date;
            var endExclusive = toDate.Date.AddDays(1);
            var totalDays = (toDate.Date - startDate).Days + 1;

            var regsByDate = GetUserRegistrationCountsByDateAction(startDate, endExclusive);

            var result = new List<int>(totalDays);
            for (int i = 0; i < totalDays; i++)
            {
                var day = startDate.AddDays(i);
                regsByDate.TryGetValue(day, out var cnt);
                result.Add(cnt);
            }

            return result;
        }

        public List<LoginLog> GetLoginLogs(int page = 1, int pageSize = 20)
        {
            return GetLoginLogsAction(page, pageSize);
        }

        public int GetLoginLogCount()
        {
            return GetLoginLogCountAction();
        }

        public List<User> GetAll(int page = 1, int pageSize = 20)
        {
            return GetAllUsersAction(page, pageSize).ToList();
        }

        public void UpdateUserRange(IEnumerable<User> users)
        {
            var userList = users.ToList();
            foreach (var user in userList)
            {
                if (!IsValidEmail(user.Email))
                {
                    throw new InvalidOperationException($"Invalid email format for user with ID: {user.Id} or Name: {user.Name}.");
                }

                if (UserExistsByEmailExcludingIdAction(user.Email, user.Id))
                {
                    throw new InvalidOperationException($"Cannot update user with ID: {user.Id}. The email '{user.Email}' is already in use by another user.");
                }

                if (UserExistsByNameExcludingIdAction(user.Name, user.Id))
                {
                    throw new InvalidOperationException($"Cannot update user with ID: {user.Id}. Another user already has this name.");
                }
            }
            
            SaveUserRangeAction(userList);
        }
    }
}