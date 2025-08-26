using FluentValidation;
using NineERP.Application.Dtos.Identity.Requests;

namespace NineERP.Application.Validator
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("RoleNotEmpty");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserNameNotEmpty")
                .MinimumLength(3).WithMessage("UserNameMinimumLength")
                .MaximumLength(100).WithMessage("UserNameMaximumLength");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("EmailNotEmpty")
                .EmailAddress().WithMessage("InvalidEmailFormat")
                .MaximumLength(100).WithMessage("EmailMaximumLength");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumberNotEmpty")
                .Matches(@"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-69]|9\d)(\d{7})$").WithMessage("InvalidPhoneNumberFormat");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullNameNotEmpty")
                .MinimumLength(2).WithMessage("FullNameMinimumLength")
                .MaximumLength(255).WithMessage("FullNameMaximumLength");
        }
    }
}
