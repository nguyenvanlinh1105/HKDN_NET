using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities
{
    public class AuditLog : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(256)]
        public string TableName { get; set; } = default!;

        [StringLength(128)]
        public string? UserId { get; set; }

        [StringLength(256)]
        public string? UserName { get; set; }

        [Required]
        [StringLength(20)]
        public string ActionType { get; set; } = default!; // Insert, Update, Delete

        public DateTime ActionTimestamp { get; set; }

        [StringLength(2048)]
        public string? KeyValues { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        [StringLength(256)]
        public string? IpAddress { get; set; }
    }
}
