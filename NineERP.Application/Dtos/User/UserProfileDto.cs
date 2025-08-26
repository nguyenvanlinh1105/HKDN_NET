namespace NineERP.Application.Dtos.User
{
    public class UserProfileDto
    {
        public string Username { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UserInfoDto
    {
        public string FullName { get; set; } = default!;
        public string NickName { get; set; } = default!;
        public string EmployeeNo { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PositionVi { get; set; }
        public string? PositionEn { get; set; }
        public string? PositionJp { get; set; }
        public string? Role { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; } // 0: Nam, 1: Nu, 2: Khac
        public short? MaritalStatus { get; set; }
        public short? NumberOfChildren { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime? ProvideDate { get; set; }
        public string? ProvidePlace { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Address { get; set; }
    }
}
