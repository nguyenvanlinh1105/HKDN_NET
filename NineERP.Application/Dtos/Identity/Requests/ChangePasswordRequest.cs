using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.Identity.Requests
{
    public class ChangePasswordRequest
    {
        [Required] public string Password { get; set; } = default!;

        [Required] public string NewPassword { get; set; } = default!;

        [Required] public string ConfirmNewPassword { get; set; } = default!;
    }
}