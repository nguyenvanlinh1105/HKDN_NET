using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatTask : AuditableBaseEntity<int>
    {
        public int ProjectId { get; set; }
        public int? ParentId { get; set; }
        public short TypeTaskId { get; set; }
        public short StatusTaskId { get; set; }
        public short PriorityTaskId { get; set; }
        public string TitleTask { get; set; } = default!;
        public decimal? Duration { get; set; }
        public decimal? Estimate { get; set; }
        public short? Completed { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Assigned { get; set; }
        public int TaskNo { get; set; }
    }
}
