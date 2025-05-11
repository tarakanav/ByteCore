using System;

namespace ByteCore.Domain.UserScope
{
    public class AuditLog
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
        public DateTime ActionTime { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        public string HttpMethod { get; set; }
        public string UrlAccessed { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public string QueryString { get; set; }
        public string FormData { get; set; }
        public int ResponseStatusCode { get; set; }
        public long? ResponseContentLength { get; set; }
        public int? ExecutionTimeMs { get; set; }
    }
}