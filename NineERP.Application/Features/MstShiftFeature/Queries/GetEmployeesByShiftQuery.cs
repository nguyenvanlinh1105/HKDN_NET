using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstShiftFeature.Queries;
public record GetEmployeesByShiftQuery(int ShiftId) : IRequest<IResult<List<EmployeeInShiftDto>>>;

public class GetEmployeesByShiftQueryHandler : IRequestHandler<GetEmployeesByShiftQuery, IResult<List<EmployeeInShiftDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeesByShiftQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<List<EmployeeInShiftDto>>> Handle(GetEmployeesByShiftQuery request, CancellationToken cancellationToken)
    {
        var employees = await _context.DatEmployeeShifts.AsNoTracking()
            .Where(x => x.ShiftId == request.ShiftId && !x.IsDeleted)
            .Join(
                _context.DatEmployees.Where(e => !e.IsDeleted),
                shift => shift.EmployeeNo,
                emp => emp.EmployeeNo,
                (shift, emp) => new EmployeeInShiftDto
                {
                    EmployeeCode = emp.EmployeeNo,
                    FullName = emp.FullName
                })
            .ToListAsync(cancellationToken);

        return await Result<List<EmployeeInShiftDto>>.SuccessAsync(employees);
    }
}