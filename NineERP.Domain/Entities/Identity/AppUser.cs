using NineERP.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace NineERP.Domain.Entities.Identity
{
	public class AppUser : IdentityUser<string>, IAuditableEntity<string>
	{
        public override string? Email { get; set; }
        public override string? PhoneNumber { get; set; }
        public string FullName { get; set; } = default!;
		public string? AvatarUrl { get; set; }
		public string CreatedBy { get; set; } = default!;
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }
		public bool IsDeleted { get; set; }
    }
}