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
        public void SaveLog(AuditLog log)
        {
            using (var context = new ApplicationDbContext())
            {
                context.AuditLogs.Add(log);
                context.SaveChanges();
            }
        }

        public IEnumerable<AuditLog> GetAll(int page = 1, int pageSize = 20)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.AuditLogs
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        public int GetLogCount()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.AuditLogs.Count();
            }
        }
    }
}