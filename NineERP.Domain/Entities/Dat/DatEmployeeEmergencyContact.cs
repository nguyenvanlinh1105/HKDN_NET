using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeEmergencyContact : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        [MaxLength(255)]
        public string? NamePrimaryContact { get; set; }
        [MaxLength(255)]
        public string? RelationshipPrimaryContact { get; set; }
        [MaxLength(255)]
        public string? PhoneNoPrimaryContact { get; set; }
        [MaxLength(255)]
        public string? NameSecondaryContact { get; set; }
        [MaxLength(255)]
        public string? RelationshipSecondaryContact { get; set; }
        [MaxLength(255)]
        public string? PhoneNoSecondaryContact { get; set; }
    }
}
