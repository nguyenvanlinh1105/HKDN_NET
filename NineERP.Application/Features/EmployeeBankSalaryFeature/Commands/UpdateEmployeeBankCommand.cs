using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.BankSalary;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeBankSalaryFeature.Commands
{
    public record UpdateEmployeeBankCommand(BankDto Bank) : IRequest<GenericResponse<object>>
    {
        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService)
            : IRequestHandler<UpdateEmployeeBankCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(UpdateEmployeeBankCommand request, CancellationToken cancellationToken)
            {
                var salary = await context.DatEmployeeSalaries
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.EmployeeNo == currentUserService.EmployeeNo, cancellationToken);
                if (salary == null) return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("BS001"), "BS001", ErrorMessages.GetMessage("BS001"));

                salary.BankAccountName = request.Bank.BankAccountName;
                salary.BankName = request.Bank.BankName;
                salary.BankNumber = request.Bank.BankNumber;
                salary.PaymentType = request.Bank.PaymentType;

                await context.SaveChangesAsync(cancellationToken);
                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0001"), "");
            }
        }
    }
}
