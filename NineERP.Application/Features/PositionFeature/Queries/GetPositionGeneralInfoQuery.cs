using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.PositionGeneral;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.PositionFeature.Queries
{
    public record GetPositionGeneralInfoQuery : IRequest<GenericResponse<PositionGeneralDto>>
    {
        public string EmployeeNo { get; set; } = default!;

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetPositionGeneralInfoQuery, GenericResponse<PositionGeneralDto>>
        {
            public async Task<GenericResponse<PositionGeneralDto>> Handle(
                GetPositionGeneralInfoQuery request, CancellationToken cancellationToken)
            {
                var query = await (from e in context.DatEmployees.AsNoTracking()
                                   where !e.IsDeleted && e.EmployeeNo == request.EmployeeNo
                                   join mst in context.MstDepartments.AsNoTracking() on e.DepartmentId equals mst.Id into deptTemp
                                   from dept in deptTemp.DefaultIfEmpty()
                                   join kbnContractType in context.KbnContractTypes.AsNoTracking() on e.ContractTypeId
                                       equals kbnContractType.Id into contractTypeTemp
                                   from contractType in contractTypeTemp.DefaultIfEmpty()
                                   join kbnEmployeeStatus in context.KbnEmployeeStatus.AsNoTracking() on
                                       e.EmployeeStatusId equals kbnEmployeeStatus.Id into employeeStatusTemp
                                   from employeeStatus in employeeStatusTemp.DefaultIfEmpty()
                                   select new PositionGeneralDto
                                   {
                                       ContractFrom = e.ContractFrom,
                                       WorkingDateFrom = e.WorkingDateFrom,
                                       WorkingDateTo = e.WorkingDateTo,
                                       TeamNameEn = dept != null ? dept.NameEn : null,
                                       TeamNameJp = dept != null ? dept.NameJa : null,
                                       TeamNameVi = dept != null ? dept.NameVi : null,
                                       ContractTypeNameEn = contractType != null ? contractType.NameEn : null,
                                       ContractTypeNameJp = contractType != null ? contractType.NameJp : null,
                                       ContractTypeNameVi = contractType != null ? contractType.NameVi : null,
                                       EmployeeStatusEn = employeeStatus != null ? employeeStatus.NameEn : null,
                                       EmployeeStatusJp = employeeStatus != null ? employeeStatus.NameJp : null,
                                       EmployeeStatusVi = employeeStatus != null ? employeeStatus.NameVi : null,
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (query == null)
                    return GenericResponse<PositionGeneralDto>.ErrorResponse(400, ErrorMessages.GetMessage("PGI001"),
                        "PGI001", ErrorMessages.GetMessage("PGI001"));

                return GenericResponse<PositionGeneralDto>.SuccessResponse(200, string.Empty, query);
            }
        }
    }
}
