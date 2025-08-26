using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Queries;
public record GetAllMstProgrammingLanguageQuery : IRequest<IResult<List<MstProgrammingLanguageDto>>>;
public class GetAllMstProgrammingLanguageQueryHandler : IRequestHandler<GetAllMstProgrammingLanguageQuery, IResult<List<MstProgrammingLanguageDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllMstProgrammingLanguageQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<MstProgrammingLanguageDto>>> Handle(GetAllMstProgrammingLanguageQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.MstProgrammingLanguages.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<MstProgrammingLanguageDto>>(list);
        return await Result<List<MstProgrammingLanguageDto>>.SuccessAsync(result);
    }
}
