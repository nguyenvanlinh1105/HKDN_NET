using FluentValidation;
using NineERP.Application.Features.EmployeesFeature.Commands;

namespace NineERP.Validators.EmployeesFeature
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.Employee.FullName)
            .NotEmpty().WithMessage("Full name must not be empty.");

            RuleFor(x => x.Employee.Email)
            .NotEmpty().WithMessage("Email must not be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Employee.EmployeeNo)
            .NotEmpty().WithMessage("Employee number must not be empty.");

            RuleFor(x => (int)(x.Employee.PositionId ?? 0))
            .GreaterThan(0).WithMessage("Invalid position.");

            RuleFor(x => (int)(x.Employee.DepartmentId   ?? 0))
            .GreaterThan(0).WithMessage("Invalid department.");
        }
    }
}