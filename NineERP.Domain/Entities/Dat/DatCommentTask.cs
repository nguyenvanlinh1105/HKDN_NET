using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatCommentTask : AuditableBaseEntity<int>
    {
        public int TaskId { get; set; }
        public string Comment { get; set; } = default!;
    }
}
