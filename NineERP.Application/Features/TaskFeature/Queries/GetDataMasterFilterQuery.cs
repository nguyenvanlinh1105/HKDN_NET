using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.TaskFeature.Queries
{
    public record GetDataMasterFilterQuery : IRequest<GenericResponse<DataMasterFilterDto>>
    {
        public int ProjectId { get; set; }
        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetDataMasterFilterQuery, GenericResponse<DataMasterFilterDto>>
        {
            public async Task<GenericResponse<DataMasterFilterDto>> Handle(GetDataMasterFilterQuery request, CancellationToken cancellationToken)
            {
                var types = await context.MstTypeTasks.AsNoTracking().Where(x => x.ProjectId == request.ProjectId)
                    .Select(x => new DataFilterDto
                    {
                        Id = x.Id,
                        NameEn = x.NameEn,
                        NameJa = x.NameJa,
                        NameVi = x.NameVi,
                    }).ToListAsync(cancellationToken);

                var status = await context.MstStatusTasks.AsNoTracking().Where(x => x.ProjectId == request.ProjectId)
                    .Select(x => new DataFilterDto
                    {
                        Id = x.Id,
                        NameEn = x.NameEn,
                        NameJa = x.NameJa,
                        NameVi = x.NameVi,
                    }).ToListAsync(cancellationToken);

                var priorities = await context.MstPriorityTasks.AsNoTracking().Where(x => x.ProjectId == request.ProjectId)
                    .Select(x => new DataFilterDto
                    {
                        Id = x.Id,
                        NameEn = x.NameEn,
                        NameJa = x.NameJa,
                        NameVi = x.NameVi,
                    }).ToListAsync(cancellationToken);

                var users = await context.DatEmployees.AsNoTracking().Where(x => !x.IsDeleted)
                .Select(x => new DataFilterUserDto
                {
                    EmployeeNo = x.EmployeeNo,
                    NickName = x.NickName,
                }).ToListAsync(cancellationToken);

                var result = new DataMasterFilterDto
                {
                    Status = status,
                    Users = users,
                    Priorities = priorities,
                    Types = types
                };

                return GenericResponse<DataMasterFilterDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
