using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstShiftFeature.Queries;
public record GetAllMstShiftQuery : IRequest<IResult<List<MstShiftDto>>>;
public class GetAllMstShiftQueryHandler : IRequestHandler<GetAllMstShiftQuery, IResult<List<MstShiftDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllMstShiftQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<MstShiftDto>>> Handle(GetAllMstShiftQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.MstShifts
        .AsNoTracking()
        .Where(x => !x.IsDeleted)
        .Select(x => new MstShiftDto
        {
            Id = x.Id,
            MorningStartTime = x.MorningStartTime,
            MorningEndTime = x.MorningEndTime,
            AfternoonStartTime = x.AfternoonStartTime,
            AfternoonEndTime = x.AfternoonEndTime,
            Description = x.Description,
            IsDefault = x.IsDefault,
            NumberOfEmployee = _context.DatEmployeeShifts
                .Count(es => es.ShiftId == x.Id && !es.IsDeleted
                             && _context.DatEmployees.Any(emp => emp.EmployeeNo == es.EmployeeNo && !emp.IsDeleted))
        })
        .ToListAsync(cancellationToken);

        return await Result<List<MstShiftDto>>.SuccessAsync(result);
    }
}
