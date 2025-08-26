using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Queries;
public record GetKbnLeaveTypeByIdQuery(int Id) : IRequest<IResult<KbnLeaveTypeDto>>;
public class GetKbnLeaveTypeByIdQueryHandler : IRequestHandler<GetKbnLeaveTypeByIdQuery, IResult<KbnLeaveTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetKbnLeaveTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<KbnLeaveTypeDto>> Handle(GetKbnLeaveTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnLeaveTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<KbnLeaveTypeDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<KbnLeaveTypeDto>(entity);
        return await Result<KbnLeaveTypeDto>.SuccessAsync(dto);
    }
}