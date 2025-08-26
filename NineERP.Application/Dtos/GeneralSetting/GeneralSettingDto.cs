namespace NineERP.Application.Dtos.GeneralSetting
{
    public class GeneralSettingDto
    {
        public string? CompanyName { get; set; }
        public string? ShortName { get; set; }
        public string? TaxCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? PasswordDefault { get; set; }
        public string? FiscalYearStartDay { get; set; }
        public short AnnualLeaveDays { get; set; }
        public string? ContractNumber { get; set; }
        public string? ApprovedBy { get; set; }
        public string? CancelBy { get; set; }
        public string? AccountHolder { get; set; }
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
