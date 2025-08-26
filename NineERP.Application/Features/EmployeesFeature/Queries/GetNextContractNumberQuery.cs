using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.Contracts;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetNextContractNumberQuery(string ContractType) : IRequest<IResult<NextContractNoDto>>;

public class GetNextContractNumberQueryHandler : IRequestHandler<GetNextContractNumberQuery, IResult<NextContractNoDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNextContractNumberQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<NextContractNoDto>> Handle(GetNextContractNumberQuery request, CancellationToken cancellationToken)
    {
        var groupCode = request.ContractType?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(groupCode) ||
            !ContractNoConstants.GroupCodeToNumberKeyMap.TryGetValue(groupCode, out var numberKey) ||
            !ContractNoConstants.GroupCodeToPrefixKeyMap.TryGetValue(groupCode, out var prefixKey))
        {
            return await Result<NextContractNoDto>.FailAsync("Invalid contract group code.");
        }

        var numberEntry = await _context.KbnConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == numberKey && !x.IsDeleted, cancellationToken);

        if (numberEntry == null)
        {
            return await Result<NextContractNoDto>.FailAsync($"Missing number config for group: {groupCode}");
        }

        var prefixEntry = await _context.KbnConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == prefixKey && !x.IsDeleted, cancellationToken);

        if (prefixEntry == null)
        {
            return await Result<NextContractNoDto>.FailAsync($"Missing prefix config for group: {groupCode}");
        }

        var nextNumber = numberEntry.IntValue ?? 0;
        var prefixText = prefixEntry.TextValue ?? string.Empty;

        var result = new NextContractNoDto
        {
            Prefix = prefixText,
            Number = nextNumber.ToString("D4")
        };

        return await Result<NextContractNoDto>.SuccessAsync(result);
    }
}
