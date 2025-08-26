using FluentValidation;
using NineERP.Application.Dtos.Identity.Requests;

namespace NineERP.Application.Validators.UsersFeature
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
        }
    }

}
