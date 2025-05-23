using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Interfaces;
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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                return Regex.IsMatch(email,
                    @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)"
                    + @"(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

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
                EnrolledCourses = new List<UserCourse>(),
                RegistrationTime = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                LatestBrowserUsed = browser
            };

            if (newUser.Name == "admin")
                newUser.Role = "Admin";

            _db.Users.Add(newUser);
            var loginLog = CreateLoginLog(newUser, ip, userAgent);
            newUser.LoginLogs.Add(loginLog);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public User AuthenticateUser(
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

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (browser != null)
            {
                user.LatestBrowserUsed = browser;
            }
            
            var loginLog = CreateLoginLog(user, ip, userAgent);
            user.LoginLogs.Add(loginLog);
            _db.SaveChanges();

            return user;
        }
        
        private LoginLog CreateLoginLog(User user, string ip, string userAgent)
        {
            var log = new LoginLog
            {
                User = user,
                IpAddress = ip,
                UserAgent = userAgent
            };

            return log;
        }

        public User GetUserByEmail(string email)
        {
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            return _db.Users
                .Include(u => u.EnrolledCourses.Select(c => c.Course.Chapters))
                .Include(u => u.CompletedChapters)
                .FirstOrDefault(u => u.Email == email);
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

            var user = _db.Users.FirstOrDefault(u => u.Email == currentEmail);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            
            if (!user.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) && _db.Users.Any(u => u.Email == updatedUser.Email && u.Id != user.Id))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
            
            if (_db.Users.Any(u => u.Name == updatedUser.Name && u.Id != user.Id))
            {
                throw new InvalidOperationException("User with this name already exists.");
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            await _db.SaveChangesAsync();

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

            var session = await _db.UserSessions.FirstOrDefaultAsync(x => x.User.Email == email);
            var expireTime = rememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(1);
            if (session == null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
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
                .Include(x => x.User.LastSeenHistory)
                .FirstOrDefault(x => x.CookieString == cookie && x.ExpireTime >= DateTime.UtcNow);

            if (session?.User.LastSeen < DateTime.UtcNow.AddMinutes(-5))
            {
                session.User.LastSeen = DateTime.UtcNow;
                _db.SaveChanges();
            }

            if (session?.User != null && (session.User.LastSeenHistory == null || !session.User.LastSeenHistory.Select(x => x.SeenOn).Contains(DateTime.Today)))
            {
                if (session.User.LastSeenHistory == null)
                {
                    session.User.LastSeenHistory = new List<UserLastSeen>();
                }
                session.User.LastSeenHistory.Add(new UserLastSeen
                {
                    SeenOn = DateTime.Today,
                    User = session.User
                });
                _db.SaveChanges();
            }

            return session?.User;
        }

        public int GetUserCount()
        {
            return _db.Users.Count();
        }

        public User GetFirstUser()
        {
            return _db.Users.OrderBy(x => x.Id).FirstOrDefault();
        }

        public User GetLastUser()
        {
            return _db.Users.OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public Dictionary<string, int> GetBrowserUsages()
        {
            using (var context = new ApplicationDbContext())
            {
                var browserCounts = context.Users
                    .Where(u => !string.IsNullOrEmpty(u.LatestBrowserUsed))
                    .GroupBy(u => u.LatestBrowserUsed)
                    .Select(g => new
                    {
                        Browser = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                var total = browserCounts.Sum(x => x.Count);
                if (total == 0)
                    return new Dictionary<string, int>();

                var result = browserCounts.ToDictionary(
                    x => x.Browser,
                    x => (int)Math.Round((double)x.Count * 100 / total)
                );

                return result;
            }
        }

        public List<int> GetActiveUserCount(DateTime fromDate, DateTime toDate)
        {
            var dates = new List<DateTime>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            var users = _db.Users
                .Where(x => x.LastSeenHistory
                    .Any(y => y.SeenOn >= fromDate.Date && y.SeenOn <= toDate.Date))
                .Include(user => user.LastSeenHistory)
                .ToList();

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

            var regsByDate = _db.Users
                .Where(u => u.RegistrationTime >= startDate
                            && u.RegistrationTime < endExclusive)
                .GroupBy(u => DbFunctions.TruncateTime(u.RegistrationTime))
                .Select(g => new
                {
                    Date = g.Key.Value,
                    Count = g.Count()
                })
                .ToDictionary(x => x.Date, x => x.Count);

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
            return _db.LoginLogs
                .Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetLoginLogCount()
        {
            return _db.LoginLogs.Count();
        }

        public IEnumerable<User> GetAll(int page = 1, int pageSize = 20)
        {
            return _db.Users
                .Include(x => x.EnrolledCourses.Select(c => c.Course.Chapters))
                .Include(x => x.CompletedChapters)
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void UpdateUserRange(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (!IsValidEmail(user.Email))
                {
                    throw new InvalidOperationException($"Invalid email format for user with ID: {user.Id} or Name: {user.Name}.");
                }

                var existingUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    if (!existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) && _db.Users.Any(u => u.Email == user.Email && u.Id != user.Id))
                    {
                        throw new InvalidOperationException($"Cannot update user with ID: {user.Id}. The email '{user.Email}' is already in use by another user.");
                    }

                    if (_db.Users.Any(u => u.Name == user.Name && u.Id != user.Id))
                    {
                        throw new InvalidOperationException($"Cannot update user with ID: {user.Id}. Another user already has this name.");
                    }

                    existingUser.Name = user.Name;
                    existingUser.Email = user.Email;
                    existingUser.Role = user.Role;
                }
            }

            _db.SaveChanges();
        }
    }
}