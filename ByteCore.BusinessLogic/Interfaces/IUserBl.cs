using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface IUserBl
    {
        Task<User> RegisterUserAsync(string name, string email, string password, string browser = null, string ip = null, string userAgent = null);
        User AuthenticateUser(string email, string password, string browser = null, string ip = null, string userAgent = null);
        User GetUserByEmail(string email);
        Task<User> UpdateUserAsync(string currentEmail, User updatedUser);
        Task<HttpCookie> GetUserCookieAsync(string email, bool rememberMe);
        User GetUserByCookie(string cookie);
        int GetUserCount();
        User GetFirstUser();
        User GetLastUser();
        Dictionary<string, int> GetBrowserUsages();
        List<int> GetActiveUserCount(DateTime fromDate, DateTime toDate);
        List<int> GetUserCountByRegistrationDate(DateTime fromDate, DateTime toDate);
    }
}