using FluentValidation;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.BankSalary;

namespace NineERP.Application.Validator
{
    public class BankDtoValidator : AbstractValidator<BankDto>
    {
        public BankDtoValidator()
        {
            RuleFor(x => x.BankName)
                .MaximumLength(255).WithErrorCode("BA001").WithMessage(ErrorMessages.GetMessage("BA001"));

            RuleFor(x => x.BankNumber)
                .MaximumLength(30).WithErrorCode("BA002").WithMessage(ErrorMessages.GetMessage("BA002"));

            RuleFor(x => x.BankAccountName)
                .MaximumLength(255).WithErrorCode("BA003").WithMessage(ErrorMessages.GetMessage("BA003"));

            RuleFor(x => x.PaymentType)
                .InclusiveBetween((short)0, (short)1)
                .WithErrorCode("BA004").WithMessage(ErrorMessages.GetMessage("BA004"));

            When(x => x.PaymentType == 1, () =>
            {
                RuleFor(x => x.BankName)
                    .NotEmpty().WithErrorCode("BA005").WithMessage(ErrorMessages.GetMessage("BA005"));

                RuleFor(x => x.BankNumber)
                    .NotEmpty().WithErrorCode("BA006").WithMessage(ErrorMessages.GetMessage("BA006"));

                RuleFor(x => x.BankAccountName)
                    .NotEmpty().WithErrorCode("BA007").WithMessage(ErrorMessages.GetMessage("BA007"));
            });
        }
    }
}