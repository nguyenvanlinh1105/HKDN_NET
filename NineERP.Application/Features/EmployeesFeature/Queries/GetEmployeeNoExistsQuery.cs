using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetEmployeeNoExistsQuery(string EmployeeNo) : IRequest<IResult<bool>>;

public class GetEmployeeNoExistsQueryHandler : IRequestHandler<GetEmployeeNoExistsQuery, IResult<bool>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeNoExistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<bool>> Handle(GetEmployeeNoExistsQuery request, CancellationToken cancellationToken)
    {
        var exists = await _context.DatEmployees
            .AsNoTracking()
            .AnyAsync(x => x.EmployeeNo == request.EmployeeNo && !x.IsDeleted, cancellationToken);

        return await Result<bool>.SuccessAsync(exists);
    }
}
