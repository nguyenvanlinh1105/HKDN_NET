using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Commands;

public record DeleteKbnEmployeeStatusCommand(int Id) : IRequest<IResult>;

public class DeleteKbnEmployeeStatusCommandHandler : IRequestHandler<DeleteKbnEmployeeStatusCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteKbnEmployeeStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteKbnEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnEmployeeStatus.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.KbnEmployeeStatus.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
