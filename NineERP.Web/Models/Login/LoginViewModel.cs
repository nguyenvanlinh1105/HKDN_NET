using System.ComponentModel.DataAnnotations;

namespace NineERP.Web.Models.Login
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:""<>?|])[A-Za-z\d!@#$%^&*()_+:""<>?|]{8,}$")]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }
    }
}
