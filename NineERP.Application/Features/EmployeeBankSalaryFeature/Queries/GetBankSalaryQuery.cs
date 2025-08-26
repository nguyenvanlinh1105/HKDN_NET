using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.BankSalary;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeBankSalaryFeature.Queries
{
    public class GetBankSalaryQuery : IRequest<GenericResponse<BankSalaryDto>>
    {
        public string EmployeeNo { get; set; } = default!;

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetBankSalaryQuery, GenericResponse<BankSalaryDto>>
        {
            public async Task<GenericResponse<BankSalaryDto>> Handle(GetBankSalaryQuery request, CancellationToken cancellationToken)
            {
                var query = await (from ec in context.DatEmployeeSalaries.AsNoTracking()
                                   where !ec.IsDeleted && ec.EmployeeNo == request.EmployeeNo
                                   select new BankSalaryDto
                                   {
                                      BankAccountName = ec.BankAccountName,
                                      BankName = ec.BankName,
                                      BankNumber = ec.BankNumber,
                                      PaymentType = ec.PaymentType,
                                      SalaryBasic = ec.SalaryBasic,
                                      SalaryGross = ec.SalaryGross
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return GenericResponse<BankSalaryDto>.ErrorResponse(400, ErrorMessages.GetMessage("BS003"), "EI011", ErrorMessages.GetMessage("BS003"));

                return GenericResponse<BankSalaryDto>.SuccessResponse(200, string.Empty, query);
            }
        }
    }
}
