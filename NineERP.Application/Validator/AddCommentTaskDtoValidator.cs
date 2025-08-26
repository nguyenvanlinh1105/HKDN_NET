using FluentValidation;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Task;

namespace NineERP.Application.Validator
{
    public class AddCommentTaskDtoValidator : AbstractValidator<AddCommentTaskDto>
    {
        public AddCommentTaskDtoValidator()
        {
            RuleFor(x => x.TaskId)
                .GreaterThan(0)
                .WithErrorCode("TC001")
                .WithMessage(ErrorMessages.GetMessage("TC001"));

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithErrorCode("TC002")
                .WithMessage(ErrorMessages.GetMessage("TC002"))
                .MaximumLength(1000)
                .WithErrorCode("TC003")
                .WithMessage(ErrorMessages.GetMessage("TC003"));
        }
    }
}