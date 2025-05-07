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

        public async Task<User> RegisterUserAsync(string name, string email, string password, string browser = null)
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
                EnrolledCourses = new List<UserCourse>(),
                RegistrationTime = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                LatestBrowserUsed = browser
            };

            if (newUser.Name == "admin")
                newUser.Role = "Admin";

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public User AuthenticateUser(string email, string password, string browser = null)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (browser != null)
            {
                user.LatestBrowserUsed = browser;
                _db.SaveChanges();
            }

            return user;
        }

        public User GetUserByEmail(string email)
        {
            return _db.Users
                .Include(u => u.EnrolledCourses.Select(c => c.Course.Chapters))
                .Include(u => u.CompletedChapters)
                .FirstOrDefault(u => u.Email == email);
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
                .Include(x => x.User.LastSeenHistory)
                .FirstOrDefault(x => x.CookieString == cookie && x.ExpireTime >= DateTime.UtcNow);

            if (session?.User.LastSeen < DateTime.UtcNow.AddMinutes(-5))
            {
                session.User.LastSeen = DateTime.UtcNow;
                _db.SaveChanges();
            }

            if (!session?.User.LastSeenHistory.Select(x => x.SeenOn).Contains(DateTime.Today) ?? false)
            {
                if (session.User.LastSeenHistory != null)
                    session.User.LastSeenHistory.Add(new UserLastSeen
                    {
                        SeenOn = DateTime.Today,
                        User = session.User
                    });
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
    }
}