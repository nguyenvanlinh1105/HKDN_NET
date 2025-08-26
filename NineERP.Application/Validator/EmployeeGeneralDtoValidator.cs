using FluentValidation;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Employees;

namespace NineERP.Application.Validator
{
    public class EmployeeGeneralDtoValidator : AbstractValidator<EmployeeGeneralDto>
    {
        public EmployeeGeneralDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithErrorCode("E015").WithMessage(ErrorMessages.GetMessage("E015"))
                .MaximumLength(255).WithErrorCode("E016").WithMessage(ErrorMessages.GetMessage("E016"));

            RuleFor(x => x.NickName)
                .NotEmpty().WithErrorCode("E017").WithMessage(ErrorMessages.GetMessage("E017"))
                .MaximumLength(255).WithErrorCode("E018").WithMessage(ErrorMessages.GetMessage("E018"));

            RuleFor(x => x.PhoneNo)
                .MaximumLength(15).WithErrorCode("E022").WithMessage(ErrorMessages.GetMessage("E022"))
                .Matches(@"^\+?\d{0,15}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNo))
                    .WithErrorCode("E023").WithMessage(ErrorMessages.GetMessage("E023"));

            RuleFor(x => x.Birthday)
                .LessThan(DateTime.Today).WithErrorCode("E024").WithMessage(ErrorMessages.GetMessage("E024"))
                .When(x => x.Birthday.HasValue);

            RuleFor(x => x.Gender)
                .InclusiveBetween((byte)0, (byte)2).WithErrorCode("E025").WithMessage(ErrorMessages.GetMessage("E025"))
                .When(x => x.Gender.HasValue);

            RuleFor(x => x.MaritalStatus)
                .InclusiveBetween((short)0, (short)2).WithErrorCode("E026").WithMessage(ErrorMessages.GetMessage("E026"))
                .When(x => x.MaritalStatus.HasValue);

            RuleFor(x => x.NumberChild)
                .GreaterThanOrEqualTo((short)0).WithErrorCode("E027").WithMessage(ErrorMessages.GetMessage("E027"))
                .When(x => x.NumberChild.HasValue);

            RuleFor(x => x.IdentityCard)
                .MaximumLength(255).WithErrorCode("E028").WithMessage(ErrorMessages.GetMessage("E028"));

            RuleFor(x => x.ProvideDateIdentityCard)
                .LessThanOrEqualTo(DateTime.Today).WithErrorCode("E029").WithMessage(ErrorMessages.GetMessage("E029"))
                .When(x => x.ProvideDateIdentityCard.HasValue);

            RuleFor(x => x.ProvidePlaceIdentityCard)
                .MaximumLength(255).WithErrorCode("E030").WithMessage(ErrorMessages.GetMessage("E030"));

            RuleFor(x => x.PlaceOfBirth)
                .MaximumLength(255).WithErrorCode("E031").WithMessage(ErrorMessages.GetMessage("E031"));

            RuleFor(x => x.Address)
                .MaximumLength(255).WithErrorCode("E032").WithMessage(ErrorMessages.GetMessage("E032"));
        }
    }

}
