using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public IEnumerable<AuditLog> GetAll(int page = 1, int pageSize = 20)
        {
            var logs = _context.AuditLogs
                .Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return logs;
        }

        public int GetLogCount()
        {
            var count = _context.AuditLogs.Count();
            return count;
        }
    }
}