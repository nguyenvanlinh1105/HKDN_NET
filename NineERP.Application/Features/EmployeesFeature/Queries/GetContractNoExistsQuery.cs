using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetContractNoExistsQuery(string ContractNo) : IRequest<IResult<bool>>;

public class GetContractNoExistsQueryHandler : IRequestHandler<GetContractNoExistsQuery, IResult<bool>>
{
    private readonly IApplicationDbContext _context;

    public GetContractNoExistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<bool>> Handle(GetContractNoExistsQuery request, CancellationToken cancellationToken)
    {
        var exists = await _context.DatEmployees
            .AsNoTracking()
            .AnyAsync(x => x.ContractNumber == request.ContractNo && !x.IsDeleted, cancellationToken);

        return await Result<bool>.SuccessAsync(exists);
    }
}
