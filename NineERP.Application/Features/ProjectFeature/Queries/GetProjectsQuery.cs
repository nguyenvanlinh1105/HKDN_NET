using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Project;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.ProjectFeature.Queries
{
    public record GetProjectsQuery : IRequest<GenericResponse<ProjectsDto>>
    {
        public string? Keyword { get; set; }
        public List<short>? Type { get; set; }
        public List<short>? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService)
            : IRequestHandler<GetProjectsQuery, GenericResponse<ProjectsDto>>
        {
            public async Task<GenericResponse<ProjectsDto>> Handle(
                GetProjectsQuery request, CancellationToken cancellationToken)
            {
                var query = context.DatProjects.AsNoTracking()
                    .Where(z => !z.IsDeleted
                             && (string.IsNullOrEmpty(request.Keyword) || EF.Functions.Like(z.ProjectName, $"%{request.Keyword}%"))
                             && (request.Type == null || request.Type.Contains(z.ProjectType))
                             && (request.Status == null || request.Status.Contains(z.StatusProject))
                                && (!request.StartDate.HasValue || request.StartDate!.Value.Date <= z.CreatedOn.Date)
                                && (!request.EndDate.HasValue || request.EndDate!.Value.Date <= z.CreatedOn.Date))
                    .OrderByDescending(z => z.Id)
                    .GroupJoin(
                        context.DatProjectDetails.AsNoTracking()
                            .Join(context.DatEmployees.AsNoTracking(),
                                pd => pd.EmployeeNo,
                                e => e.EmployeeNo,
                                (pd, e) => new { pd, e })
                            .Select(x => new
                            {
                                x.pd.Id,
                                x.pd.ProjectId,
                                x.pd.EmployeeNo,
                                x.pd.JoinDate,
                                x.e.ImageURL,
                            }).Take(3),
                        p => p.Id,
                        d => d.ProjectId,
                        (p, details) => new { p, details })
                    .Where(l => l.details.Select(i => i.EmployeeNo).Contains(currentUserService.EmployeeNo))
                    .Select(x => new ProjectDto
                    {
                        Id = x.p.Id,
                        Name = x.p.ProjectName,
                        ProjectType = x.p.ProjectType,
                        StatusProject = x.p.StatusProject,
                        TotalParticipants = x.details.Any() ? x.details.Count() : 0,
                        ImageTopThreeParticipants = x.details.Select(d => d.ImageURL).ToList()
                    });

                var totalRecords = await query.CountAsync(cancellationToken);
                var projects = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
                var result = new ProjectsDto
                {
                    Projects = projects,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<ProjectsDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
