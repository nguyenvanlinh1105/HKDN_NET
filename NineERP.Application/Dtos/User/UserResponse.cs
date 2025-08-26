namespace NineERP.Application.Dtos.User
{
    public class UserResponse
    {
        public string Id { get; set; } = default!;
        public string? Email { get; set; }
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public bool LockoutEnabled { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
