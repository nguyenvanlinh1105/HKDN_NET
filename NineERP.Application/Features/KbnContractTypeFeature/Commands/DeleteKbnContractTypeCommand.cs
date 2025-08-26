using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.KbnContractTypeFeature.Commands;

public record DeleteKbnContractTypeCommand(int Id) : IRequest<IResult>;

public class DeleteContractTypeCommandHandler : IRequestHandler<DeleteKbnContractTypeCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteContractTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteKbnContractTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnContractTypes.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.KbnContractTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
