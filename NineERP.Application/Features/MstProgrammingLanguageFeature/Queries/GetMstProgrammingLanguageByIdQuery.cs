using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Queries;
public record GetMstProgrammingLanguageByIdQuery(int Id) : IRequest<IResult<MstProgrammingLanguageDto>>;
public class GetMstProgrammingLanguageByIdQueryHandler : IRequestHandler<GetMstProgrammingLanguageByIdQuery, IResult<MstProgrammingLanguageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetMstProgrammingLanguageByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<MstProgrammingLanguageDto>> Handle(GetMstProgrammingLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstProgrammingLanguages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<MstProgrammingLanguageDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<MstProgrammingLanguageDto>(entity);
        return await Result<MstProgrammingLanguageDto>.SuccessAsync(dto);
    }
}