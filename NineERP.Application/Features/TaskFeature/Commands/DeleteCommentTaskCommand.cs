using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.TaskFeature.Commands
{
    public class DeleteCommentTaskCommand : IRequest<GenericResponse<object>>
    {
        public int Id { get; set; }

        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService) : IRequestHandler<DeleteCommentTaskCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(DeleteCommentTaskCommand request, CancellationToken cancellationToken)
            {
                var comment = await context.DatCommentTasks.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (comment == null) return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("TC004"), "TC004", ErrorMessages.GetMessage("TC004"));

                if (!comment.CreatedBy.Equals(currentUserService.EmployeeNo)) return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("TC005"), "TC005", ErrorMessages.GetMessage("TC005"));

                comment.IsDeleted = true;
                context.DatCommentTasks.Update(comment);
                await context.SaveChangesAsync(cancellationToken);

                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0006"), null!);
            }
        }
    }
}
