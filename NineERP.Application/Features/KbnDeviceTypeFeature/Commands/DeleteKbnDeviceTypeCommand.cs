using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.KbnDeviceTypeFeature.Commands;

public record DeleteKbnDeviceTypeCommand(int Id) : IRequest<IResult>;

public class DeleteDeviceTypeCommandHandler : IRequestHandler<DeleteKbnDeviceTypeCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteDeviceTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteKbnDeviceTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnDeviceTypes.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.KbnDeviceTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
