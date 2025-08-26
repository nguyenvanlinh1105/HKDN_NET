using FluentValidation;
using NineERP.Application.Features.UsersFeature.Commands;
namespace NineERP.Application.Validators.UsersFeature
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Model.Password)
            .NotEmpty().WithMessage("Current password must not be empty")
            .MinimumLength(6).WithMessage("Minimum length is 6 characters");

            RuleFor(x => x.Model.NewPassword)
            .NotEmpty().WithMessage("New password must not be empty")
            .MinimumLength(6).WithMessage("Minimum length is 6 characters")
            .Matches("[A-Z]").WithMessage("Must contain at least one uppercase letter")
            .Matches("[0-9]").WithMessage("Must contain at least one digit");

            RuleFor(x => x.Model.ConfirmNewPassword)
            .Equal(x => x.Model.NewPassword).WithMessage("Confirmation password does not match");
        }
    }
}