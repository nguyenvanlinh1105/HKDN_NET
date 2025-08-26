using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeEducationInfo : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public string? Education { get; set; }
        public string? BusinessExperience { get; set; }
        public string? SoftSkills { get; set; }
        public string? EnglishLevel { get; set; }
        public string? JapaneseLevel { get; set; }
    }
}
