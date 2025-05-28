using System.Collections.Generic;
using ByteCore.BusinessLogic.APIs;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class AuditLogBl : AuditLogApi, IAuditLogBl
    {
        public void SaveLog(AuditLog log)
        {
            SaveLogAction(log);
        }

        public IEnumerable<AuditLog> GetAll(int page = 1, int pageSize = 20)
        {
            return GetAllLogsAction(page, pageSize);
        }

        public int GetLogCount()
        {
            return GetLogCountAction();
        }
    }
}