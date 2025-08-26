using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Commands;
public record CreateMstProgrammingLanguageCommand(MstProgrammingLanguageDto MstProgrammingLanguage) : IRequest<IResult>
{
    public class CreateDeviceTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateMstProgrammingLanguageCommand, IResult>
    {
        public async Task<IResult> Handle(CreateMstProgrammingLanguageCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo MstProgrammingLanguage
                var mstProgrammingLanguage = mapper.Map<MstProgrammingLanguage>(request.MstProgrammingLanguage);
                mstProgrammingLanguage.CreatedOn = DateTime.Now;
                context.MstProgrammingLanguages.Add(mstProgrammingLanguage);

                // 2. Lưu vào database
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return await Result.FailAsync($"Create failed: {ex.Message}");
            }
        }
    }
}
