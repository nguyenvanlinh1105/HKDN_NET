using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.Identity.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}