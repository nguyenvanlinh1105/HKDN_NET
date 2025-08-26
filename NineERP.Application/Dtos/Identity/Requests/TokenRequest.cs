using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.Identity.Requests
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}