using System.ComponentModel.DataAnnotations;

namespace NineERP.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:""<>?|])[A-Za-z\d!@#$%^&*()_+:""<>?|]{8,}$")]
        public string Password { get; set; } = default!;
    }
}
