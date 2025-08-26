using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.TaskFeature.Commands
{
    public class AddCommentCommand : IRequest<GenericResponse<object>>
    {
        public int TaskId { get; set; }
        public string Comment { get; set; } = default!;

        public class Handler(IApplicationDbContext context) : IRequestHandler<AddCommentCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
            {
                var addComment = new DatCommentTask
                {
                    TaskId = request.TaskId,
                    Comment = request.Comment,
                };

                await context.DatCommentTasks.AddAsync(addComment, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0003"), null!);
            }
        }
    }
}
