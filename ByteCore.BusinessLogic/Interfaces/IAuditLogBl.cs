using System.Collections.Generic;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface IAuditLogBl
    {
        void SaveLog(AuditLog log);
        List<AuditLog> GetAll(int page = 1, int pageSize = 20);
        int GetLogCount();
    }
}