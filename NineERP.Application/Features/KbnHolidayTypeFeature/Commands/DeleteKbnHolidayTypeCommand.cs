using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Commands;

public record DeleteKbnHolidayTypeCommand(int Id) : IRequest<IResult>;

public class DeleteKbnHolidayTypeCommandHandler : IRequestHandler<DeleteKbnHolidayTypeCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteKbnHolidayTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteKbnHolidayTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnHolidayTypes.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.KbnHolidayTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
