using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatProject : AuditableBaseEntity<int>
    {
        [MaxLength(255)]
        public string ProjectName { get; set; } = default!;
        public short ProjectType { get; set; } // 1: Internal, 2: External, 3: Fix price
        public short CustomerId { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Technology { get; set; }
        public string? Note { get; set; }
        public short StatusProject { get; set; } // 1: New, 2: In progress, 3: Completed, 4: Cancelled, 5: Closed
        public string? ProjectUrl { get; set; }
    }
}
