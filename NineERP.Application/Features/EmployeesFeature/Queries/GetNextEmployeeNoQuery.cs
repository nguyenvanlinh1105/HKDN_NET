using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetNextEmployeeNoQuery(bool IsIntern) : IRequest<IResult<NextEmployeeNoDto>>;

public class GetNextEmployeeNoQueryHandler : IRequestHandler<GetNextEmployeeNoQuery, IResult<NextEmployeeNoDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNextEmployeeNoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<NextEmployeeNoDto>> Handle(GetNextEmployeeNoQuery request, CancellationToken cancellationToken)
    {
        var prefixCode = request.IsIntern ? EmployeeNoConstants.InternPrefixKey : EmployeeNoConstants.EmployeePrefixKey;
        var numberCode = request.IsIntern ? EmployeeNoConstants.InternNumberKey : EmployeeNoConstants.EmployeeNumberKey;

        var prefixEntry = await _context.KbnConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == prefixCode && !x.IsDeleted, cancellationToken);

        var numberEntry = await _context.KbnConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == numberCode && !x.IsDeleted, cancellationToken);

        if (prefixEntry == null || numberEntry == null)
        {
            return await Result<NextEmployeeNoDto>.FailAsync("Employee number configuration not found.");
        }

        var number = numberEntry.IntValue ?? 0;
        var result = new NextEmployeeNoDto
        {
            Prefix = prefixEntry.TextValue ?? "",
            Number = number.ToString("D4")
        };

        return await Result<NextEmployeeNoDto>.SuccessAsync(result);
    }
}


