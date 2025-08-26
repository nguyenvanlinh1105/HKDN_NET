using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Features.MstDepartmentsFeature.Queries;

public record GetDepartmentTreeQuery : IRequest<IResult<List<DepartmentTreeDto>>>;

public class GetDepartmentTreeQueryHandler : IRequestHandler<GetDepartmentTreeQuery, IResult<List<DepartmentTreeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetDepartmentTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<List<DepartmentTreeDto>>> Handle(GetDepartmentTreeQuery request, CancellationToken cancellationToken)
    {
        var departments = await _context.MstDepartments
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var tree = BuildTree(null, departments);
        return await Result<List<DepartmentTreeDto>>.SuccessAsync(tree);
    }

    private List<DepartmentTreeDto> BuildTree(int? parentId, List<MstDepartment> departments)
    {
        return departments
            .Where(d => d.ParentId == parentId)
            .Select(d => new DepartmentTreeDto
            {
                Id = d.Id,
                NameVi = d.NameVi,
                NameEn = d.NameEn,
                NameJa = d.NameJa,
                Children = BuildTree(d.Id, departments)
            })
            .ToList();
    }
}
