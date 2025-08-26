using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities
{
    public class EmailTemplate : AuditableBaseEntity<int>
    {
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = default!; // e.g., CONFIRM_EMAIL, RESET_PASSWORD

        [Required]
        [StringLength(10)]
        public string Language { get; set; } = "en"; // e.g., en, vi

        [Required]
        [StringLength(255)]
        public string Subject { get; set; } = default!;

        [Required]
        public string Body { get; set; } = default!; // HTML content

        public bool IsActive { get; set; } = true;

    }
}
