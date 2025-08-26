using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetEmployeeStatisticsQuery : IRequest<IResult<EmployeeStatisticsDto>>;

public class GetEmployeeStatisticsQueryHandler : IRequestHandler<GetEmployeeStatisticsQuery, IResult<EmployeeStatisticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeStatisticsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<EmployeeStatisticsDto>> Handle(GetEmployeeStatisticsQuery request, CancellationToken cancellationToken)
    {
        var query = from e in _context.DatEmployees.AsNoTracking()
                    join ct in _context.KbnContractTypes.AsNoTracking()
                        on e.ContractTypeId equals ct.Id
                    where !e.IsDeleted && !ct.IsDeleted
                    select new { e.Id, ct.GroupCode };

        var total = await query.CountAsync(cancellationToken);

        var official = await query.CountAsync(x => x.GroupCode == "OFFICIAL", cancellationToken);
        var probation = await query.CountAsync(x => x.GroupCode == "PROBATION", cancellationToken);
        var intern = await query.CountAsync(x => x.GroupCode == "INTERNSHIP", cancellationToken);
        var temporary = await query.CountAsync(x => x.GroupCode == "TEMPORARY", cancellationToken);
        var flexible = await query.CountAsync(x => x.GroupCode == "FLEXIBLE", cancellationToken);
        var project = await query.CountAsync(x => x.GroupCode == "PROJECT", cancellationToken);

        var result = new EmployeeStatisticsDto
        {
            Total = total,
            Official = official,
            Probation = probation,
            Intern = intern,
            Temporary = temporary,
            Flexible = flexible,
            Project = project
        };

        return await Result<EmployeeStatisticsDto>.SuccessAsync(result);
    }
}
