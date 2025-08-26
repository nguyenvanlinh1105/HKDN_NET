using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatDocumentTask : AuditableBaseEntity<int>
    {
        public int TaskId { get; set; }
        public string NameFile { get; set; } = default!;
        public string TypeFile { get; set; } = default!;
        public string SizeFile { get; set; } = default!;
        public string FileUrl { get; set; } = default!;
    }
}
