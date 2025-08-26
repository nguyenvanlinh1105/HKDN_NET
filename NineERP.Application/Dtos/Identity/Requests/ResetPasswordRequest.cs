using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.Identity.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        public string Token { get; set; } = default!;

        public bool ResetFlag { get; set; } = true;
    }
}