using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ERP.Domain.Entities.DatTable.Employee
{
    public class DatEmployeeIdentity : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;

        // Chỉ dùng CCCD
        [MaxLength(20)]
        public string? CitizenshipCard { get; set; }

        public DateTime? ProvideDateCitizenshipCard { get; set; }

        [MaxLength(100)]
        public string? ProvidePlaceCitizenshipCard { get; set; }

        [MaxLength(255)]
        public string? PhotoBeforeCitizenshipCard { get; set; }

        [MaxLength(255)]
        public string? PhotoAfterCitizenshipCard { get; set; }
    }
}
