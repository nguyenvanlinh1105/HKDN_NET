using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Project;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.ProjectFeature.Queries
{
    public record GetParticipantsQuery : IRequest<GenericResponse<ParticipantsDto>>
    {
        public string? Keyword { get; set; }
        public List<short>? Roles { get; set; }
        public List<short>? Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetParticipantsQuery, GenericResponse<ParticipantsDto>>
        {
            public async Task<GenericResponse<ParticipantsDto>> Handle(
                GetParticipantsQuery request, CancellationToken cancellationToken)
            {
                var query = from pd in context.DatProjectDetails.AsNoTracking()
                            where !pd.IsDeleted && pd.ProjectId == request.ProjectId
                            join e in context.DatEmployees.AsNoTracking() on pd.EmployeeNo equals e.EmployeeNo
                            join mstP in context.MstPositions.AsNoTracking() on e.PositionId equals mstP.Id into positions
                            from p in positions.DefaultIfEmpty()
                            where (string.IsNullOrEmpty(request.Keyword) || EF.Functions.Like(pd.EmployeeNo, $"%{request.Keyword}%") || EF.Functions.Like(e.NickName, $"%{request.Keyword}%"))
                                    && (request.Roles == null || request.Roles.Contains(p.Id))
                                    && (request.Status == null || request.Status.Contains(pd.Status))
                                    && (!request.StartDate.HasValue || request.StartDate.Value.Date <= pd.JoinDate.Date)
                                    && (!request.EndDate.HasValue || request.EndDate.Value.Date >= pd.JoinDate.Date)
                            select new ParticipantDto
                            {
                                EmployeeNo = pd.EmployeeNo,
                                NickName = e.NickName,
                                ImageUrl = e.ImageURL,
                                JoinDate = pd.JoinDate,
                                Status = pd.Status,
                                RoleNameVi = p != null ? p.NameVi : null,
                                RoleNameEn = p != null ? p.NameEn : null,
                                RoleNameJa = p != null ? p.NameJa : null
                            };

                var pagedQuery = query.OrderByDescending(x => x.JoinDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                var totalRecords = await query.CountAsync(cancellationToken);
                var participants = await pagedQuery.ToListAsync(cancellationToken);

                var result = new ParticipantsDto
                {
                    Participants = participants,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<ParticipantsDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
