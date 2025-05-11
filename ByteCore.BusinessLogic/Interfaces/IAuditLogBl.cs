using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface IAuditLogBl
    {
        void SaveLog(AuditLog log);
    }
}