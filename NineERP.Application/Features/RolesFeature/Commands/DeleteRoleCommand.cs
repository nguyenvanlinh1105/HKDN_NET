using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.RolesFeature.Commands;

public class DeleteRoleCommand(string id) : IRequest<IResult>
{
    public string Id { get; set; } = id;

    public class Handler(IApplicationDbContext context) : IRequestHandler<DeleteRoleCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await context.Roles.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);

            if (role == null) return await Result.FailAsync(MessageConstants.NotFound);

            role.IsDeleted = true;
            context.Roles.Update(role);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
        }
    }
}