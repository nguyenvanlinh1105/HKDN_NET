using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EmergencyContact;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeEmergencyFeature.Queries
{
    public class GetEmergencyContactQuery : IRequest<GenericResponse<EmergencyContactDto>>
    {
        public string EmployeeNo { get; set; } = default!;

        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetEmergencyContactQuery, GenericResponse<EmergencyContactDto>>
        {
            public async Task<GenericResponse<EmergencyContactDto>> Handle(GetEmergencyContactQuery request, CancellationToken cancellationToken)
            {
                var query = await (from ec in context.DatEmployeeEmergencyContacts.AsNoTracking()
                                   where !ec.IsDeleted && ec.EmployeeNo == request.EmployeeNo
                                   select new EmergencyContactDto
                                   {
                                       NamePrimaryContact = ec.NamePrimaryContact,
                                       NameSecondaryContact = ec.NameSecondaryContact,
                                       PhoneNoPrimaryContact = ec.PhoneNoPrimaryContact,
                                       PhoneNoSecondaryContact = ec.PhoneNoSecondaryContact,
                                       RelationshipPrimaryContact = ec.RelationshipPrimaryContact,
                                       RelationshipSecondaryContact = ec.RelationshipSecondaryContact,
                                   }).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return GenericResponse<EmergencyContactDto>.ErrorResponse(400, ErrorMessages.GetMessage("EC015"), "EC015", ErrorMessages.GetMessage("EC015"));

                return GenericResponse<EmergencyContactDto>.SuccessResponse(200, string.Empty, query);
            }
        }
    }
}
