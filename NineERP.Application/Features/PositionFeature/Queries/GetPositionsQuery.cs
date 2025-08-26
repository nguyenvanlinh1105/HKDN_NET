using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.PositionGeneral;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.PositionFeature.Queries
{
    public record GetPositionsQuery : IRequest<GenericResponse<List<PositionDto>>>
    {
        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetPositionsQuery, GenericResponse<List<PositionDto>>>
        {
            public async Task<GenericResponse<List<PositionDto>>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
            {
                var positions = await context.MstPositions.AsNoTracking().Where(x => !x.IsDeleted).Select(x => new PositionDto
                {
                    Id = x.Id,
                    NameVi = x.NameVi,
                    NameEn = x.NameEn,
                    NameJa = x.NameJa,
                }).ToListAsync(cancellationToken);

                var result = positions.Any() ? positions : new List<PositionDto>();

                return GenericResponse<List<PositionDto>>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
