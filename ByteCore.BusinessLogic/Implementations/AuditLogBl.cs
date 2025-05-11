using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class AuditLogBl : IAuditLogBl
    {
        private readonly ApplicationDbContext _context;

        public AuditLogBl(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SaveLog(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }
}