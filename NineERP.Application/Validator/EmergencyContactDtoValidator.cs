using FluentValidation;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EmergencyContact;

namespace NineERP.Application.Validator
{
    public class EmergencyContactDtoValidator : AbstractValidator<EmergencyContactDto>
    {
        public EmergencyContactDtoValidator()
        {
            // Primary Contact
            RuleFor(x => x.NamePrimaryContact)
                .NotEmpty().WithErrorCode("EC001").WithMessage(ErrorMessages.GetMessage("EC001"))
                .MaximumLength(255).WithErrorCode("EC002").WithMessage(ErrorMessages.GetMessage("EC002"));

            RuleFor(x => x.RelationshipPrimaryContact)
                .NotEmpty().WithErrorCode("EC011").WithMessage(ErrorMessages.GetMessage("EC011"))
                .MaximumLength(255).WithErrorCode("EC012").WithMessage(ErrorMessages.GetMessage("EC012"));

            RuleFor(x => x.PhoneNoPrimaryContact)
                .NotEmpty().WithErrorCode("EC003").WithMessage(ErrorMessages.GetMessage("EC003"))
                .MaximumLength(15).WithErrorCode("EC004").WithMessage(ErrorMessages.GetMessage("EC004"))
                .Matches(@"^\+?\d{0,15}$").WithErrorCode("EC005").WithMessage(ErrorMessages.GetMessage("EC005"));

            // Secondary Contact (optional)
            RuleFor(x => x.NameSecondaryContact)
                .MaximumLength(255).WithErrorCode("EC007").WithMessage(ErrorMessages.GetMessage("EC007"))
                .When(x => !string.IsNullOrWhiteSpace(x.NameSecondaryContact));

            RuleFor(x => x.RelationshipSecondaryContact)
                .MaximumLength(255).WithErrorCode("EC014").WithMessage(ErrorMessages.GetMessage("EC014"))
                .When(x => !string.IsNullOrWhiteSpace(x.RelationshipSecondaryContact));

            RuleFor(x => x.PhoneNoSecondaryContact)
                .MaximumLength(15).WithErrorCode("EC009").WithMessage(ErrorMessages.GetMessage("EC009"))
                .Matches(@"^\+?\d{0,15}$").WithErrorCode("EC010").WithMessage(ErrorMessages.GetMessage("EC010"))
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNoSecondaryContact));
        }
    }
}
