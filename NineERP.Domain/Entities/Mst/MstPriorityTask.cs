using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Mst
{
    public class MstPriorityTask : AuditableBaseEntity<short>
    {
        public int ProjectId { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameVi { get; set; } = default!;
        public string NameJa { get; set; } = default!;
        public string? Description { get; set; }
        public string? Color { get; set; }
    }
}
