using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Kbn
{
    public class KbnConfiguration : AuditableBaseEntity<short>
    {
        public required string Code { get; set; }
        public int? IntValue { get; set; }
        public string? TextValue { get; set; }
    }
}