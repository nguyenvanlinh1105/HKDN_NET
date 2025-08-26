using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EducationInfo;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.EmployeeEducationInfoFeature.Commands
{
    public record UpdateEducationInfoCommand(EducationInfoDto EducationInfo)
        : IRequest<GenericResponse<object>>
    {
        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService)
            : IRequestHandler<UpdateEducationInfoCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
            {
                var educationInfo = await context.DatEmployeeEducationInfos
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.EmployeeNo == currentUserService.EmployeeNo, cancellationToken);
                if (educationInfo == null)
                {
                    var educationInfoAdd = new DatEmployeeEducationInfo
                    {
                        EmployeeNo = currentUserService.EmployeeNo,
                        BusinessExperience = request.EducationInfo.BusinessExperience,
                        Education = request.EducationInfo.Education,
                        EnglishLevel = request.EducationInfo.EnglishLevel,
                        JapaneseLevel = request.EducationInfo.JapaneseLevel,
                        SoftSkills = request.EducationInfo.SoftSkills,
                    };
                    await context.DatEmployeeEducationInfos.AddAsync(educationInfoAdd, cancellationToken);
                }
                else
                {
                    educationInfo.BusinessExperience = request.EducationInfo.BusinessExperience;
                    educationInfo.Education = request.EducationInfo.Education;
                    educationInfo.EnglishLevel = request.EducationInfo.EnglishLevel;
                    educationInfo.JapaneseLevel = request.EducationInfo.JapaneseLevel;
                    educationInfo.SoftSkills = request.EducationInfo.SoftSkills;
                    context.DatEmployeeEducationInfos.Update(educationInfo);
                }

                await context.SaveChangesAsync(cancellationToken);

                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0001"), "");
            }
        }
    }
}
