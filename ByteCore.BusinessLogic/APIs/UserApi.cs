using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.UserScope;
using ByteCore.Helpers;

namespace ByteCore.BusinessLogic.APIs
{
    public class UserApi
    {
        internal bool UserExistsByEmailOrNameAction(string email, string name)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Any(u => u.Email == email || u.Name == name);
            }
        }

        internal bool UserExistsByEmailExcludingIdAction(string email, int userId)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Any(u => u.Email == email && u.Id != userId);
            }
        }

        internal bool UserExistsByNameExcludingIdAction(string name, int userId)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Any(u => u.Name == name && u.Id != userId);
            }
        }

        internal async Task AddUserAction(User newUser)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Users.Add(newUser);
                await db.SaveChangesAsync();
            }
        }

        internal User GetUserByCookieAction(string cookie)
        {
            using (var db = new ApplicationDbContext())
            {
                var session = db.UserSessions
                    .Include(x => x.User.LastSeenHistory)
                    .FirstOrDefault(x => x.CookieString == cookie && x.ExpireTime >= DateTime.UtcNow);

                if (session?.User.LastSeen < DateTime.UtcNow.AddMinutes(-5))
                {
                    session.User.LastSeen = DateTime.UtcNow;
                    db.SaveChanges();
                }

                if (session?.User != null && (session.User.LastSeenHistory == null ||
                                              !session.User.LastSeenHistory.Select(x => x.SeenOn)
                                                  .Contains(DateTime.Today)))
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
                    db.SaveChanges();
                }

                return session?.User;
            }
        }

        internal User GetUserByEmailAction(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Include(u => u.EnrolledCourses.Select(c => c.Course.Chapters))
                    .Include(u => u.CompletedChapters)
                    .FirstOrDefault(u => u.Email == email);
            }
        }

        internal User GetUserByEmailSimpleAction(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
        }
        
        protected bool IsValidEmail(string email)
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

        internal User AuthenticateUserAction(
            string email,
            string password,
            string browser = null,
            string ip = null,
            string userAgent = null)
        {
            using (var db = new ApplicationDbContext())
            {
                if (!IsValidEmail(email))
                {
                    throw new InvalidOperationException("Invalid email format.");
                }

                var user = db.Users.FirstOrDefault(u => u.Email == email);
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
                db.SaveChanges();

                return user;
            }
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

        internal async Task SaveUserAction(User user)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Users.Attach(user);
                db.Entry(user).State = EntityState.Modified;

                foreach (var log in user.LoginLogs.Where(l => l.Id == 0))
                {
                    db.LoginLogs.Add(log);
                }

                await db.SaveChangesAsync();
            }
        }

        internal void SaveUserRangeAction(IEnumerable<User> users)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var user in users)
                {
                    var existingUser = db.Users.Find(user.Id);
                    if (existingUser != null)
                    {
                        existingUser.Name = user.Name;
                        existingUser.Email = user.Email;
                        existingUser.Role = user.Role;
                    }
                }

                db.SaveChanges();
            }
        }

        internal async Task<UserSession> GetUserSessionAction(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return await db.UserSessions.FirstOrDefaultAsync(x => x.User.Email == email);
            }
        }

        internal async Task AddUserSessionAction(UserSession session)
        {
            using (var db = new ApplicationDbContext())
            {
                if (session.User != null && session.User.Id != 0)
                {
                    db.Users.Attach(session.User);
                }

                db.UserSessions.Add(session);
                await db.SaveChangesAsync();
            }
        }

        internal async Task UpdateUserSessionAction(UserSession session)
        {
            using (var db = new ApplicationDbContext())
            {
                db.UserSessions.Attach(session);
                db.Entry(session).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        internal int GetUserCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Count();
            }
        }

        internal User GetFirstUserAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.OrderBy(x => x.Id).FirstOrDefault();
            }
        }

        internal User GetLastUserAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.OrderByDescending(x => x.Id).FirstOrDefault();
            }
        }

        internal Dictionary<string, int> GetBrowserUsagesAction()
        {
            using (var db = new ApplicationDbContext())
            {
                var browserCounts = db.Users
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

        internal List<User> GetActiveUsersByDateRangeAction(DateTime fromDate, DateTime toDate)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Where(x => x.LastSeenHistory
                        .Any(y => y.SeenOn >= fromDate.Date && y.SeenOn <= toDate.Date))
                    .Include(user => user.LastSeenHistory)
                    .ToList();
            }
        }

        internal Dictionary<DateTime, int> GetUserRegistrationCountsByDateAction(DateTime startDate,
            DateTime endExclusive)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Where(u => u.RegistrationTime >= startDate && u.RegistrationTime < endExclusive)
                    .GroupBy(u => DbFunctions.TruncateTime(u.RegistrationTime))
                    .Select(g => new
                    {
                        Date = g.Key.Value,
                        Count = g.Count()
                    })
                    .ToDictionary(x => x.Date, x => x.Count);
            }
        }

        internal List<LoginLog> GetLoginLogsAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.LoginLogs
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        internal int GetLoginLogCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.LoginLogs.Count();
            }
        }

        internal IEnumerable<User> GetAllUsersAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Include(x => x.EnrolledCourses.Select(c => c.Course.Chapters))
                    .Include(x => x.CompletedChapters)
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }
    }
}