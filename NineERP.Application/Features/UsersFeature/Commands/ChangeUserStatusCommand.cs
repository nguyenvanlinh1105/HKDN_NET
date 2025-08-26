using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.UsersFeature.Commands;

public record ChangeUserStatusCommand(string Id) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context)
        : IRequestHandler<ChangeUserStatusCommand, IResult>
    {
        public async Task<IResult> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);
            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            user.LockoutEnabled = !user.LockoutEnabled;
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}
