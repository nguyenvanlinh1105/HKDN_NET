namespace NineERP.Application.Dtos.User
{
    public class UserDetailDto
    {
        public string Id { get; set; } = default!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
        public string? ParentId { get; set; }
    }
}
