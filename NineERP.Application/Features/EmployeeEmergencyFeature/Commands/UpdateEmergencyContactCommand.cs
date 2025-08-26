using MediatR;
using NineERP.Application.Dtos.EmergencyContact;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;
using NineERP.Domain.Entities.Dat;
using NineERP.Application.Constants.Messages;

namespace NineERP.Application.Features.EmployeeEmergencyFeature.Commands
{
    public record UpdateEmergencyContactCommand(EmergencyContactDto EmergencyContact)
        : IRequest<GenericResponse<object>>
    {
        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService)
            : IRequestHandler<UpdateEmergencyContactCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
            {
                var emergencyContact = await context.DatEmployeeEmergencyContacts
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.EmployeeNo == currentUserService.EmployeeNo, cancellationToken);
                if (emergencyContact == null)
                {
                    var emergencyContactAdd = new DatEmployeeEmergencyContact
                    {
                        EmployeeNo = currentUserService.EmployeeNo,
                        NamePrimaryContact = request.EmergencyContact.NamePrimaryContact,
                        NameSecondaryContact = request.EmergencyContact.NameSecondaryContact,
                        PhoneNoPrimaryContact = request.EmergencyContact.PhoneNoPrimaryContact,
                        PhoneNoSecondaryContact = request.EmergencyContact.PhoneNoSecondaryContact,
                        RelationshipPrimaryContact = request.EmergencyContact.RelationshipPrimaryContact,
                        RelationshipSecondaryContact = request.EmergencyContact.RelationshipSecondaryContact,
                    };
                    await context.DatEmployeeEmergencyContacts.AddAsync(emergencyContactAdd, cancellationToken);
                }
                else
                {
                    emergencyContact.NamePrimaryContact = request.EmergencyContact.NamePrimaryContact;
                    emergencyContact.NameSecondaryContact = request.EmergencyContact.NameSecondaryContact;
                    emergencyContact.PhoneNoPrimaryContact = request.EmergencyContact.PhoneNoPrimaryContact;
                    emergencyContact.PhoneNoSecondaryContact = request.EmergencyContact.PhoneNoSecondaryContact;
                    emergencyContact.RelationshipPrimaryContact = request.EmergencyContact.RelationshipPrimaryContact;
                    emergencyContact.RelationshipSecondaryContact = request.EmergencyContact.RelationshipSecondaryContact;
                    context.DatEmployeeEmergencyContacts.Update(emergencyContact);
                }

                await context.SaveChangesAsync(cancellationToken);

                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0001"), "");
            }
        }
    }
}
