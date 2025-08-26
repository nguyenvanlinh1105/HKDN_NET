using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstDepartmentsFeature.Queries;

public record GetAllDepartmentsQuery : IRequest<IResult<List<DepartmentDetailDto>>>;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, IResult<List<DepartmentDetailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<List<DepartmentDetailDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _context.MstDepartments
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<DepartmentDetailDto>>(departments);
        return await Result<List<DepartmentDetailDto>>.SuccessAsync(result);
    }
}
