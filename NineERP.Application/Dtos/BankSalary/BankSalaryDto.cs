namespace NineERP.Application.Dtos.BankSalary
{
    public class BankSalaryDto
    {
        public double? SalaryBasic { get; set; }
        public double? SalaryGross { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? BankAccountName { get; set; }
        public short PaymentType { get; set; } // 0: Cash, 1: Bank Transfer
    }

    public class BankDto
    {
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? BankAccountName { get; set; }
        public short PaymentType { get; set; } // 0: Cash, 1: Bank Transfer
    }
}
