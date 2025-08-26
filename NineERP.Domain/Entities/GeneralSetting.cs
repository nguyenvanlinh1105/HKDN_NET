using System.ComponentModel.DataAnnotations;
using NineERP.Domain.Entities.Base; 

namespace NineERP.Domain.Entities
{
    public class GeneralSetting : AuditableBaseEntity<int>
    {
        [MaxLength(256)]
        public string? CompanyName { get; set; }

        [MaxLength(50)]
        public string? ShortName { get; set; }

        [MaxLength(20)]
        public string? TaxCode { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(100)]
        public string? AccountHolder { get; set; }

        [MaxLength(50)]
        public string? PasswordDefault { get; set; }

        [MaxLength(20)]
        public string? FiscalYearStartDay { get; set; }

        public short AnnualLeaveDays { get; set; }

        [MaxLength(50)]
        public string? ContractNumber { get; set; }

        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        [MaxLength(100)]
        public string? CancelBy { get; set; }

        public float InsuranceCompanyPercent { get; set; }
        public float HealthInsuranceCompanyPercent { get; set; }
        public float AccidentInsuranceCompanyPercent { get; set; }
        public float UnionCompanyPercent { get; set; }

        public float InsuranceEmployeePercent { get; set; }
        public float HealthInsuranceEmployeePercent { get; set; }
        public float AccidentInsuranceEmployeePercent { get; set; }
        public float UnionEmployeePercent { get; set; }

        public float IncomeTaxPercent { get; set; }
    }
}
