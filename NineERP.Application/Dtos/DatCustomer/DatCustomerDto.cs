namespace NineERP.Application.Dtos.DatCustomer;

public class DatCustomerDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = default!;
    public short CustomerType { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public short ProvinceId { get; set; } = default!;
    public short CountryId { get; set; } = default!;
    public string? PostalCode { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? ImageUrl { get; set; }
    public short Status { get; set; }
}
