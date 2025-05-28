using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.UserScope;
using ByteCore.Helpers;

namespace ByteCore.BusinessLogic.APIs
{
    public class UserApi
    {
        internal bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                return Regex.IsMatch(email,
                    @"^(?!\.)""([^""\r\\]|\\[""\r\\])*""|"
                    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)" +
                    @"(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*(?:[a-z])$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        internal async Task<User> RegisterUserAction(User user, LoginLog log)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Users.Add(user);
                db.LoginLogs.Add(log);
                await db.SaveChangesAsync();
                return user;
            }
        }

        internal User AuthenticateUserAction(string email, string passwordHash, LoginLog log)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == passwordHash);
                if (user != null)
                {
                    db.LoginLogs.Add(log);
                    db.SaveChanges();
                }

                return user;
            }
        }

        internal User GetUserByEmailAction(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Include(u => u.EnrolledCourses.Select(ec => ec.Course.Chapters))
                    .Include(u => u.CompletedChapters)
                    .FirstOrDefault(u => u.Email == email);
            }
        }

        internal async Task<User> UpdateUserAction(User existing, User updates)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(existing.Id);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                user.Name = updates.Name;
                user.Email = updates.Email;
                user.Role = updates.Role;
                await db.SaveChangesAsync();
                return user;
            }
        }

        internal async Task<HttpCookie> GetUserCookieAction(User user, bool rememberMe)
        {
            using (var db = new ApplicationDbContext())
            {
                var cookie = new HttpCookie("X-KEY") { Value = CookieGenerator.Create(user.Email) };
                var session = await db.UserSessions.FirstOrDefaultAsync(s => s.User.Id == user.Id);
                var expireTime = rememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddHours(1);
                if (session == null)
                {
                    session = new UserSession { User = user, CookieString = cookie.Value, ExpireTime = expireTime };
                    db.UserSessions.Add(session);
                }
                else
                {
                    session.CookieString = cookie.Value;
                    session.ExpireTime = expireTime;
                }

                await db.SaveChangesAsync();
                cookie.Expires = session.ExpireTime;
                return cookie;
            }
        }

        internal User GetUserByCookieAction(string cookie)
        {
            using (var db = new ApplicationDbContext())
            {
                var session = db.UserSessions
                    .Include(s => s.User.LastSeenHistory)
                    .FirstOrDefault(s => s.CookieString == cookie && s.ExpireTime >= DateTime.UtcNow);
                return session?.User;
            }
        }

        internal int GetUserCountAction()
        {
            using (var db = new ApplicationDbContext())
                return db.Users.Count();
        }

        internal User GetFirstUserAction()
        {
            using (var db = new ApplicationDbContext())
                return db.Users.OrderBy(u => u.Id).FirstOrDefault();
        }

        internal User GetLastUserAction()
        {
            using (var db = new ApplicationDbContext())
                return db.Users.OrderByDescending(u => u.Id).FirstOrDefault();
        }

        internal Dictionary<string, int> GetBrowserUsagesAction()
        {
            using (var db = new ApplicationDbContext())
            {
                var total = db.Users.Count(u => !string.IsNullOrEmpty(u.LatestBrowserUsed));
                if (total == 0) return new Dictionary<string, int>();
                return db.Users
                    .Where(u => !string.IsNullOrEmpty(u.LatestBrowserUsed))
                    .GroupBy(u => u.LatestBrowserUsed)
                    .Select(g => new { Browser = g.Key, Count = (int)Math.Round((double)g.Count() * 100 / total) })
                    .ToDictionary(x => x.Browser, x => x.Count);
            }
        }

        internal List<int> GetActiveUserCountAction(DateTime fromDate, DateTime toDate)
        {
            using (var db = new ApplicationDbContext())
            {
                var list = new List<int>();
                for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                {
                    var count = db.UserSessions.Count(s => DbFunctions.TruncateTime(s.ExpireTime) == date);
                    list.Add(count);
                }

                return list;
            }
        }

        internal List<int> GetUserCountByRegistrationDateAction(DateTime fromDate, DateTime toDate)
        {
            using (var db = new ApplicationDbContext())
            {
                var dict = db.Users
                    .Where(u => DbFunctions.TruncateTime(u.RegistrationTime) >= fromDate.Date
                                && DbFunctions.TruncateTime(u.RegistrationTime) <= toDate.Date)
                    .GroupBy(u => DbFunctions.TruncateTime(u.RegistrationTime))
                    .Select(g => new { Date = g.Key.Value, Count = g.Count() })
                    .ToDictionary(x => x.Date, x => x.Count);
                var result = new List<int>();
                for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                {
                    dict.TryGetValue(date, out var count);
                    result.Add(count);
                }

                return result;
            }
        }

        internal List<LoginLog> GetLoginLogsAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.LoginLogs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        internal int GetLoginLogCountAction()
        {
            using (var db = new ApplicationDbContext())
                return db.LoginLogs.Count();
        }

        internal List<User> GetAllAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users
                    .Include(u => u.CompletedChapters)
                    .Include(u => u.EnrolledCourses.Select(ec => ec.Course.Chapters))
                    .OrderByDescending(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        internal void UpdateUserRangeAction(IEnumerable<User> users)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var user in users)
                {
                    var existing = db.Users.Find(user.Id);
                    if (existing != null)
                    {
                        existing.Name = user.Name;
                        existing.Email = user.Email;
                        existing.Role = user.Role;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}