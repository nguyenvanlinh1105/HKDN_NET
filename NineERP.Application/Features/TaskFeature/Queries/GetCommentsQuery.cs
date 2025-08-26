using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.TaskFeature.Queries
{
    public record GetCommentsQuery : IRequest<GenericResponse<CommentsTaskDto>>
    {
        public int TaskId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetCommentsQuery, GenericResponse<CommentsTaskDto>>
        {
            public async Task<GenericResponse<CommentsTaskDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
            {
                var query = from ct in context.DatCommentTasks.AsNoTracking()
                            join e in context.DatEmployees.AsNoTracking() on ct.CreatedBy equals e.EmployeeNo
                            where ct.TaskId == request.TaskId && !ct.IsDeleted
                            select new CommentTaskDto
                            {
                                Id = ct.Id,
                                Comment = ct.Comment,
                                EmployeeNo = e.EmployeeNo,
                                NickName = e.NickName,
                                ImageUrl = e.ImageURL,
                                CreatedDate = ct.CreatedOn
                            };

                var totalRecords = await query.CountAsync(cancellationToken);

                var pagedQuery = query
                    .OrderByDescending(t => t.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                var comments = await pagedQuery.ToListAsync(cancellationToken);

                var result = new CommentsTaskDto
                {
                    Comments = comments,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<CommentsTaskDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
