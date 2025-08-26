using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.MstShiftFeature.Commands;
public class MoveEmployeesCommandHandler : IRequestHandler<MoveEmployeesRequest, IResult>
{
    private readonly IApplicationDbContext _context;

    public MoveEmployeesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(MoveEmployeesRequest request, CancellationToken cancellationToken)
    {
        var shiftsToRemove = await _context.DatEmployeeShifts
            .Where(x => x.ShiftId == request.FromShiftId && request.EmployeeCodes.Contains(x.EmployeeNo))
            .ToListAsync(cancellationToken);

        var newShiftAssignments = request.EmployeeCodes.Select(code => new DatEmployeeShift
        {
            EmployeeNo = code,
            ShiftId = request.ToShiftId,
            CreatedOn = DateTime.UtcNow
        }).ToList();

        _context.DatEmployeeShifts.RemoveRange(shiftsToRemove);
        await _context.DatEmployeeShifts.AddRangeAsync(newShiftAssignments, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await Result.SuccessAsync();
    }
}