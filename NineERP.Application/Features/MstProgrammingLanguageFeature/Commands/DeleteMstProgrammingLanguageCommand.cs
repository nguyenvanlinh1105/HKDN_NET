using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Commands;

public record DeleteMstProgrammingLanguageCommand(int Id) : IRequest<IResult>;

public class DeleteMstProgrammingLanguageCommandHandler : IRequestHandler<DeleteMstProgrammingLanguageCommand, IResult>
{
    private readonly IApplicationDbContext _context;

    public DeleteMstProgrammingLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> Handle(DeleteMstProgrammingLanguageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstProgrammingLanguages.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null) return await Result.FailAsync(MessageConstants.NotFound);

        entity.IsDeleted = true;
        _context.MstProgrammingLanguages.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
