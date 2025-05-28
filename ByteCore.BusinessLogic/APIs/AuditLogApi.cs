using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.APIs
{
    public class AuditLogApi
    {
        internal void SaveLogAction(AuditLog log)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.AuditLogs.Add(log);
                    context.SaveChanges();
                }
            }
            catch
            {
                // ignored
            }
        }

        internal List<AuditLog> GetAllLogsAction(int page, int pageSize)
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

        internal int GetLogCountAction()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.AuditLogs.Count();
            }
        }
    }
}