using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstDepartmentsFeature.Queries;

public record GetDepartmentByIdQuery(int Id) : IRequest<IResult<DepartmentDetailDto>>;

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, IResult<DepartmentDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<DepartmentDetailDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstDepartments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null)
            return await Result<DepartmentDetailDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<DepartmentDetailDto>(entity);
        return await Result<DepartmentDetailDto>.SuccessAsync(dto);
    }
}
