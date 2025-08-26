using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.User;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.AuthFeature.Queries
{
    public class GetUserProfileQuery : IRequest<GenericResponse<UserInfoDto>>
    {
        public string Username { get; set; } = default!;

        public class Handler(IApplicationDbContext context) : IRequestHandler<GetUserProfileQuery, GenericResponse<UserInfoDto>>
        {
            public async Task<GenericResponse<UserInfoDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
            {
                var query = await (from datE in context.DatEmployees.AsNoTracking()
                                   join mstPositions in context.MstPositions.AsNoTracking()
                                       on datE.PositionId equals mstPositions.Id into tempTblPositions
                                   from mstP in tempTblPositions.DefaultIfEmpty()
                                   where !datE.IsDeleted && datE.EmployeeNo == request.Username
                                   select new UserInfoDto
                                   {
                                       FullName = datE.FullName,
                                       EmployeeNo = datE.EmployeeNo,
                                       NickName = datE.NickName,
                                       ImageUrl = datE.ImageURL,
                                       Email = datE.Email,
                                       PhoneNumber = datE.PhoneNo,
                                       PositionEn = mstP.NameEn,
                                       PositionVi = mstP.NameVi,
                                       PositionJp = mstP.NameJa,
                                       Role = string.Empty,
                                       Birthday = datE.Birthday,
                                       Address = datE.Address,
                                       Gender = datE.Gender,
                                       MaritalStatus = datE.MaritalStatus,
                                       NumberOfChildren = datE.NumberChild,
                                       PlaceOfBirth = datE.PlaceOfBirth,
                                       IdentityNumber = datE.IdentityCard,
                                       ProvideDate = datE.ProvideDateIdentityCard,
                                       ProvidePlace = datE.ProvidePlaceIdentityCard
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return GenericResponse<UserInfoDto>.ErrorResponse(400, ErrorMessages.GetMessage("E001"), "E001", ErrorMessages.GetMessage("E001"));

                return GenericResponse<UserInfoDto>.SuccessResponse(200, string.Empty, query);
            }
        }
    }
}
