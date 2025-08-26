using FluentValidation;
using NineERP.Application.Features.MstDepartmentsFeature.Commands;

namespace NineERP.Application.Validators.Departments
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Department.Id)
                .GreaterThan(0).WithMessage("Department ID is required.");

            RuleFor(x => x.Department.NameVi)
                .NotEmpty().WithMessage("Vietnamese name is required.")
                .MaximumLength(255).WithMessage("Vietnamese name must not exceed 255 characters.");

            RuleFor(x => x.Department.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(255).WithMessage("English name must not exceed 255 characters.");

            RuleFor(x => x.Department.NameJa)
                .NotEmpty().WithMessage("Japanese name is required.")
                .MaximumLength(255).WithMessage("Japanese name must not exceed 255 characters.");

            RuleFor(x => x.Department.ParentId)
                .NotNull().WithMessage("Parent department is required.");

            RuleFor(x => x.Department.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
