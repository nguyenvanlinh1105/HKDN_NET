// EmployeeDto.cs
namespace NineERP.Application.Dtos.Employees
{
    public class EmployeeDetailDto
    {
        public long Id { get; set; }
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

        public short? PositionId { get; set; }
        public string? PositionName { get; set; }
        public short? EmployeeStatusId { get; set; }
        public string? EmployeeStatusName { get; set; }
        public short? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public short? ContractTypeId { get; set; }
        public string? ContractTypeName { get; set; }

        public string? IdentityCard { get; set; }
        public DateTime? ProvideDateIdentityCard { get; set; }
        public string? ProvidePlaceIdentityCard { get; set; }

        public EmployeeEmergencyContactDto? EmergencyContact { get; set; }
        public EmployeeSalaryDto? SalaryInfo { get; set; }
        public EmployeeIdentityDto? IdentityInfo { get; set; }
        public List<EmployeeDocumentDto> Documents { get; set; } = new();
    }

    public class EmployeeEmergencyContactDto
    {
        public string? NamePrimaryContact { get; set; }
        public string? RelationshipPrimaryContact { get; set; }
        public string? PhoneNoPrimaryContact { get; set; }
        public string? NameSecondaryContact { get; set; }
        public string? RelationshipSecondaryContact { get; set; }
        public string? PhoneNoSecondaryContact { get; set; }
    }
    public class EmployeeSalaryDto
    {
        public double? SalaryBasic { get; set; }
        public double? SalaryGross { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? BankAccountName { get; set; }
        public short PaymentType { get; set; } // 0: Cash, 1: Bank Transfer
    }

    public class EmployeeIdentityDto
    {
        public string? CitizenshipCard { get; set; }
        public DateTime? ProvideDateCitizenshipCard { get; set; }
        public string? ProvidePlaceCitizenshipCard { get; set; }
        public string? PhotoBeforeCitizenshipCard { get; set; }
        public string? PhotoAfterCitizenshipCard { get; set; }
    }
    public class EmployeeDocumentDto
    {
        public long Id { get; set; }
        public string? FileName { get; set; }
        public string? NameFile { get; set; }
        public string? TypeFile { get; set; }
        public string? SizeFile { get; set; }
        public string? GoogleDriveFileId { get; set; }    
        public string? GoogleDriveFileUrl { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class EmployeeGeneralDto
    {
        public string FullName { get; set; } = default!;
        public string NickName { get; set; } = default!;
        public string? PhoneNo { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; } // 0: Nam, 1: Nu, 2: Khac
        public short? MaritalStatus { get; set; }
        public short? NumberChild { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Address { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime? ProvideDateIdentityCard { get; set; }
        public string? ProvidePlaceIdentityCard { get; set; }
    }
}
