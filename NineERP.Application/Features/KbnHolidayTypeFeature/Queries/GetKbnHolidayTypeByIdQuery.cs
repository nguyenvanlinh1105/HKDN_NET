using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Queries;
public record GetKbnHolidayTypeByIdQuery(int Id) : IRequest<IResult<KbnHolidayTypeDto>>;
public class GetKbnHolidayTypeByIdQueryHandler : IRequestHandler<GetKbnHolidayTypeByIdQuery, IResult<KbnHolidayTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetKbnHolidayTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<KbnHolidayTypeDto>> Handle(GetKbnHolidayTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnHolidayTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<KbnHolidayTypeDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<KbnHolidayTypeDto>(entity);
        return await Result<KbnHolidayTypeDto>.SuccessAsync(dto);
    }
}