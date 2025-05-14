using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.UserScope
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        public virtual User User { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public DateTime ActionTime { get; set; }

        [MaxLength(100)]
        public string ControllerName { get; set; }
        [MaxLength(100)]
        public string ActionName { get; set; }

        [MaxLength(10)]
        public string HttpMethod { get; set; }
        public string UrlAccessed { get; set; }
        [MaxLength(45)]
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public string QueryString { get; set; }
        public string FormData { get; set; }
        public int ResponseStatusCode { get; set; }
        public long? ResponseContentLength { get; set; }
        public int? ExecutionTimeMs { get; set; }
    }
}