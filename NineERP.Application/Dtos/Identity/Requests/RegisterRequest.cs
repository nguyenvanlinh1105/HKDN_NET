namespace NineERP.Application.Dtos.Identity.Requests
{
    public class RegisterRequest
    {
        public string Role { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? GeneralError { get; set; }
    }
}