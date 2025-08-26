using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Commands;

public record DeleteKbnLeaveTypeCommand(int Id) : IRequest<IResult>;

public class DeleteKbnLeaveTypeCommandHandler : IRequestHandler<DeleteKbnLeaveTypeCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteKbnLeaveTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteKbnLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnLeaveTypes.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.KbnLeaveTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
