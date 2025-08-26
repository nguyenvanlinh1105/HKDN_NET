using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EducationInfo;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeEducationInfoFeature.Queries
{
    public class GetEducationInfoQuery : IRequest<GenericResponse<EducationInfoDto>>
    {
        public string EmployeeNo { get; set; } = default!;

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetEducationInfoQuery, GenericResponse<EducationInfoDto>>
        {
            public async Task<GenericResponse<EducationInfoDto>> Handle(GetEducationInfoQuery request, CancellationToken cancellationToken)
            {
                var query = await (from ec in context.DatEmployeeEducationInfos.AsNoTracking()
                                   where !ec.IsDeleted && ec.EmployeeNo == request.EmployeeNo
                                   select new EducationInfoDto
                                   {
                                       Education = ec.Education,
                                       EnglishLevel = ec.EnglishLevel,
                                       JapaneseLevel = ec.JapaneseLevel,
                                       SoftSkills = ec.SoftSkills,
                                       BusinessExperience = ec.BusinessExperience
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return GenericResponse<EducationInfoDto>.ErrorResponse(400, ErrorMessages.GetMessage("EI011"), "EI011", ErrorMessages.GetMessage("EI011"));

                return GenericResponse<EducationInfoDto>.SuccessResponse(200, string.Empty, query);
            }
        }
    }
}
