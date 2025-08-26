using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetAllEmployeesQuery : IRequest<IResult<List<EmployeeDetailDto>>>;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IResult<List<EmployeeDetailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<List<EmployeeDetailDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _context.DatEmployees.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<EmployeeDetailDto>>(employees);
        return await Result<List<EmployeeDetailDto>>.SuccessAsync(result);
    }
}