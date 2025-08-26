using MediatR;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace NineERP.Application.Features.TaskFeature.Queries
{
    public record GetDocumentsTaskQuery : IRequest<GenericResponse<DocumentsTaskDto>>
    {
        public int TaskId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class Handler(IApplicationDbContext context) : IRequestHandler<GetDocumentsTaskQuery, GenericResponse<DocumentsTaskDto>>
        {
            public async Task<GenericResponse<DocumentsTaskDto>> Handle(GetDocumentsTaskQuery request, CancellationToken cancellationToken)
            {
                var query = from d in context.DatDocumentTasks.AsNoTracking()
                            where !d.IsDeleted && d.TaskId == request.TaskId
                            select new DocumentTaskDto
                            {
                                FileUrl = d.FileUrl,
                                Id = d.Id,
                                NameFile = d.NameFile,
                                SizeFile = d.SizeFile,
                                TypeFile = d.TypeFile,
                            };

                var totalRecords = await query.CountAsync(cancellationToken);

                var pagedQuery = query
                    .OrderByDescending(t => t.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                var documents = await pagedQuery.ToListAsync(cancellationToken);

                var result = new DocumentsTaskDto
                {
                    Documents = documents,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<DocumentsTaskDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
