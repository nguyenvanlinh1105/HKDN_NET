using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Project;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.ProjectFeature.Queries
{
    public record GetProjectQuery : IRequest<GenericResponse<ProjectDetailDto>>
    {
        public int Id { get; set; }

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetProjectQuery, GenericResponse<ProjectDetailDto>>
        {
            public async Task<GenericResponse<ProjectDetailDto>> Handle(
                GetProjectQuery request, CancellationToken cancellationToken)
            {
                var project = await (from p in context.DatProjects.AsNoTracking()
                                   join c in context.DatCustomers.AsNoTracking() on p.CustomerId equals c.Id into customerTemp
                                   from customer in customerTemp.DefaultIfEmpty()
                                   where !p.IsDeleted && p.Id == request.Id
                                   select new ProjectDetailDto
                                   {
                                       Name = p.ProjectName,
                                       ProjectType = p.ProjectType,
                                       StatusProject = p.StatusProject,
                                       Duration = p.Duration,
                                       StartDate = p.StartDate,
                                       EndDate = p.EndDate,
                                       CustomerName = customer != null ? customer.CompanyName : string.Empty,
                                       Technology = p.Technology,
                                       Note = p.Note,
                                       ImageUrl = p.ProjectUrl
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (project == null) return GenericResponse<ProjectDetailDto>.ErrorResponse(400, ErrorMessages.GetMessage("P001"), "P001", ErrorMessages.GetMessage("P001"));

                return GenericResponse<ProjectDetailDto>.SuccessResponse(200, string.Empty, project);
            }
        }
    }
}
