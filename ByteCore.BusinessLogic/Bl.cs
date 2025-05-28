using ByteCore.BusinessLogic.Implementations;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.BusinessLogic
{
    public static class Bl
    {
        public static IAdminBl GetAdminBl()
        {
            return new AdminBl();
        }
        
        public static ICourseBl GetCourseBl()
        {
            return new CourseBl();
        }
        
        public static IAuditLogBl GetAuditLogBl()
        {
            return new AuditLogBl();
        }

        public static IQuizBl GetQuizBl()
        {
            return new QuizBl();
        }
        
        public static IUserBl GetUserBl()
        {
            return new UserBl();
        }
    }
}