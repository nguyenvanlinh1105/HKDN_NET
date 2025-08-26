using FluentValidation;
using NineERP.Application.Features.EmployeesFeature.Commands;

namespace NineERP.Validators.EmployeesFeature
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Employee.Id)
                .GreaterThan(0).WithMessage("Employee ID is invalid.");

            RuleFor(x => x.Employee.FullName)
                .NotEmpty().WithMessage("Full name must not be empty.");

            RuleFor(x => x.Employee.Email)
                .NotEmpty().WithMessage("Email must not be empty.")
                .EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(x => x.Employee.EmployeeNo)
                .NotEmpty().WithMessage("Employee number must not be empty.");

            RuleFor(x => (int)x.Employee.PositionId)
                .GreaterThan(0).WithMessage("Position is invalid.");

            RuleFor(x => (int)x.Employee.DepartmentId)
                .GreaterThan(0).WithMessage("Department is invalid.");
        }
    }
}
