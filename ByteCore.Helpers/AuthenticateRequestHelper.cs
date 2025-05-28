using System;
using System.Security.Claims;
using System.Web;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Helpers
{
    public static class AuthenticateRequestHelper
    {
        public static void AuthenticateRequest(HttpContext context, IUserBl userBl)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var cookie = context.Request.Cookies["X-KEY"];
            if (cookie == null)
                return;

            if (userBl == null)
                return;

            var user = userBl.GetUserByCookie(cookie.Value);
            if (user == null)
                return;

            var identity = new ClaimsIdentity("CustomCookie");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "CustomIdentityProvider"));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

            var userClaims = new ClaimsPrincipal(identity);
            context.User = userClaims;
        }
    }
}