using AutoMapper;
using ERP.Domain.Entities.DatTable.Employee;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class UpdateEmployeeCommand : IRequest<IResult>
{
    public EmployeeDetailDto Employee { get; set; } = default!;
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var dto = request.Employee;

            // 1. Cập nhật DatEmployee
            var employee = await _context.DatEmployees
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == dto.Id, cancellationToken);

            if (employee == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            employee.EmployeeNo = dto.EmployeeNo;
            employee.FullName = dto.FullName;
            employee.Birthday = dto.Birthday;
            employee.Gender = dto.Gender;
            employee.PhoneNo = dto.PhoneNo;
            employee.Email = dto.Email;
            employee.Address = dto.Address;
            employee.PlaceOfBirth = dto.PlaceOfBirth;
            employee.WorkingDateFrom = dto.WorkingDateFrom;
            employee.WorkingDateTo = dto.WorkingDateTo;
            employee.ContractFrom = dto.ContractFrom;
            employee.ContractTo = dto.ContractTo;
            employee.EducationLevel = dto.EducationLevel;
            employee.SoftSkill = dto.SoftSkill;
            employee.BusinessExp = dto.BusinessExp;
            employee.MaritalStatus = dto.MaritalStatus;
            employee.NumberChild = dto.NumberChild;
            employee.NickName = dto.NickName;
            employee.DeviceToken = dto.DeviceToken;
            employee.ContractNumber = dto.ContractNumber;
            employee.ContractUrl = dto.ContractUrl;
            employee.TaxCode = dto.TaxCode;
            employee.SocialInsuranceNumber = dto.SocialInsuranceNumber;
            employee.NumberOfDependents = dto.NumberOfDependents;
            employee.PositionId = dto.PositionId;
            employee.EmployeeStatusId = dto.EmployeeStatusId;
            employee.ContractTypeId = dto.ContractTypeId;
            employee.DepartmentId = dto.DepartmentId;

            // ✅ Cập nhật lại vào DbContext
            _context.DatEmployees.Update(employee);

            // 2. Cập nhật DatEmployeeSalary
            if (dto.SalaryInfo is not null)
            {
                var salary = await _context.DatEmployeeSalaries
                    .FirstOrDefaultAsync(x => x.EmployeeNo == dto.EmployeeNo, cancellationToken);

                if (salary == null)
                {
                    salary = new DatEmployeeSalary
                    {
                        EmployeeNo = dto.EmployeeNo,
                        SalaryBasic = dto.SalaryInfo.SalaryBasic,
                        SalaryGross = dto.SalaryInfo.SalaryGross,
                        BankName = dto.SalaryInfo.BankName,
                        BankNumber = dto.SalaryInfo.BankNumber,
                        BankAccountName = dto.SalaryInfo.BankAccountName,
                        PaymentType = dto.SalaryInfo.PaymentType
                    };
                    await _context.DatEmployeeSalaries.AddAsync(salary, cancellationToken);
                }
                else
                {
                    salary.SalaryBasic = dto.SalaryInfo.SalaryBasic;
                    salary.SalaryGross = dto.SalaryInfo.SalaryGross;
                    salary.BankName = dto.SalaryInfo.BankName;
                    salary.BankNumber = dto.SalaryInfo.BankNumber;
                    salary.BankAccountName = dto.SalaryInfo.BankAccountName;
                    salary.PaymentType = dto.SalaryInfo.PaymentType;

                    _context.DatEmployeeSalaries.Update(salary);
                }
            }

            // 3. Cập nhật DatEmployeeIdentity
            if (dto.IdentityInfo is not null)
            {
                var identity = await _context.DatEmployeeIdentities
                    .FirstOrDefaultAsync(x => x.EmployeeNo == dto.EmployeeNo, cancellationToken);

                if (identity == null)
                {
                    identity = new DatEmployeeIdentity
                    {
                        EmployeeNo = dto.EmployeeNo,
                        CitizenshipCard = dto.IdentityInfo.CitizenshipCard,
                        ProvideDateCitizenshipCard = dto.IdentityInfo.ProvideDateCitizenshipCard,
                        ProvidePlaceCitizenshipCard = dto.IdentityInfo.ProvidePlaceCitizenshipCard,
                    };
                    await _context.DatEmployeeIdentities.AddAsync(identity, cancellationToken);
                }
                else
                {
                    identity.CitizenshipCard = dto.IdentityInfo.CitizenshipCard;
                    identity.ProvideDateCitizenshipCard = dto.IdentityInfo.ProvideDateCitizenshipCard;
                    identity.ProvidePlaceCitizenshipCard = dto.IdentityInfo.ProvidePlaceCitizenshipCard;
                    _context.DatEmployeeIdentities.Update(identity);
                }
            }

            // 4. Cập nhật DatEmployeeEmergencyContact
            if (dto.EmergencyContact is not null)
            {
                var contact = await _context.DatEmployeeEmergencyContacts
                    .FirstOrDefaultAsync(x => x.EmployeeNo == dto.EmployeeNo, cancellationToken);

                if (contact == null)
                {
                    contact = new DatEmployeeEmergencyContact
                    {
                        EmployeeNo = dto.EmployeeNo,
                        NamePrimaryContact = dto.EmergencyContact.NamePrimaryContact,
                        RelationshipPrimaryContact = dto.EmergencyContact.RelationshipPrimaryContact,
                        PhoneNoPrimaryContact = dto.EmergencyContact.PhoneNoPrimaryContact,
                        NameSecondaryContact = dto.EmergencyContact.NameSecondaryContact,
                        RelationshipSecondaryContact = dto.EmergencyContact.RelationshipSecondaryContact,
                        PhoneNoSecondaryContact = dto.EmergencyContact.PhoneNoSecondaryContact
                    };
                    await _context.DatEmployeeEmergencyContacts.AddAsync(contact, cancellationToken);
                }
                else
                {
                    contact.NamePrimaryContact = dto.EmergencyContact.NamePrimaryContact;
                    contact.RelationshipPrimaryContact = dto.EmergencyContact.RelationshipPrimaryContact;
                    contact.PhoneNoPrimaryContact = dto.EmergencyContact.PhoneNoPrimaryContact;
                    contact.NameSecondaryContact = dto.EmergencyContact.NameSecondaryContact;
                    contact.RelationshipSecondaryContact = dto.EmergencyContact.RelationshipSecondaryContact;
                    contact.PhoneNoSecondaryContact = dto.EmergencyContact.PhoneNoSecondaryContact;

                    _context.DatEmployeeEmergencyContacts.Update(contact);
                }
            }
                
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return await Result.FailAsync($"Cập nhật thất bại: {ex.Message}");
        }
    }
}
