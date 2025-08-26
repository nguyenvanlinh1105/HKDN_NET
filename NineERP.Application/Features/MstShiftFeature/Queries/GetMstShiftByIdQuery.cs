using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstShiftFeature.Queries;
public record GetMstShiftByIdQuery(int Id) : IRequest<IResult<MstShiftDto>>;
public class GetMstShiftByIdQueryHandler : IRequestHandler<GetMstShiftByIdQuery, IResult<MstShiftDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetMstShiftByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<MstShiftDto>> Handle(GetMstShiftByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstShifts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<MstShiftDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<MstShiftDto>(entity);
        return await Result<MstShiftDto>.SuccessAsync(dto);
    }
}