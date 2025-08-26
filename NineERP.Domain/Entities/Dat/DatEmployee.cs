using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployee : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; }
        public string? PhoneNo { get; set; }
        public string Email { get; set; } = default!;
        public string? Address { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? WorkingDateFrom { get; set; }
        public DateTime? WorkingDateTo { get; set; }
        public DateTime? ContractFrom { get; set; }
        public DateTime? ContractTo { get; set; }
        public string? EducationLevel { get; set; }
        public string? SoftSkill { get; set; }
        public string? BusinessExp { get; set; }
        public int? Salary { get; set; }
        public short? MaritalStatus { get; set; }
        public short? NumberChild { get; set; }
        public string? NickName { get; set; }
        public string? ChatId { get; set; }
        public string? ImageURL { get; set; }
        public string? KatakanaName { get; set; }
        public string? FamilyPhoneNo { get; set; }
        public string? DeviceToken { get; set; }
        public string? ContractNumber { get; set; }
        public string? ContractUrl { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }
        public string? NumberOfDependents { get; set; }

        // ✅ Đã sửa thành nullable để tránh lỗi bind
        public short? PositionId { get; set; }
        public short? EmployeeStatusId { get; set; }
        public short? ContractTypeId { get; set; }
        public short? DepartmentId { get; set; }

        public string? IdentityCard { get; set; }
        public DateTime? ProvideDateIdentityCard { get; set; }
        public string? ProvidePlaceIdentityCard { get; set; }
    }
}
