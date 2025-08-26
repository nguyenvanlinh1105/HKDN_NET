using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.MstTeamFeature.Commands;

public record DeleteMstTeamCommand(int Id) : IRequest<IResult>;

public class DeleteMstTeamCommandHandler : IRequestHandler<DeleteMstTeamCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteMstTeamCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteMstTeamCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstTeams.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.MstTeams.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
