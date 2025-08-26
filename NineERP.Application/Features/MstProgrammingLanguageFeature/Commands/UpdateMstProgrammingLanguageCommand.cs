using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Commands;
public record UpdateMstProgrammingLanguageCommand(MstProgrammingLanguageDto MstProgrammingLanguage) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateMstProgrammingLanguageCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateMstProgrammingLanguageCommand request, CancellationToken cancellationToken)
        {
            var mstProgrammingLanguage = await context.MstProgrammingLanguages
                .FirstOrDefaultAsync(x => x.Id == request.MstProgrammingLanguage.Id && !x.IsDeleted, cancellationToken);

            if (mstProgrammingLanguage == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            mstProgrammingLanguage = mapper.Map(request.MstProgrammingLanguage, mstProgrammingLanguage);
            mstProgrammingLanguage.LastModifiedOn = DateTime.Now;
            context.MstProgrammingLanguages.Update(mstProgrammingLanguage);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

