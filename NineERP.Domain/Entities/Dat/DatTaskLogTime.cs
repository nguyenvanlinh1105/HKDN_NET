using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatTaskLogTime : AuditableBaseEntity<int>
    {
        public int TaskId { get; set; }
        public DateTime DateLogTime { get; set; }
        public decimal Hours { get; set; }
        public string? Comment { get; set; }
        public short ActivityId { get; set; }
    }
}
