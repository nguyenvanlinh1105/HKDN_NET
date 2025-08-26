using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.MstShiftFeature.Commands;

public record DeleteMstShiftCommand(int Id) : IRequest<IResult>;

public class DeleteMstShiftCommandHandler : IRequestHandler<DeleteMstShiftCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteMstShiftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteMstShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstShifts.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.MstShifts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
