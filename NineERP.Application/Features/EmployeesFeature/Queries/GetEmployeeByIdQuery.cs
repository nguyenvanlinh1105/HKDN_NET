using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Globalization;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetEmployeeByIdQuery(long Id) : IRequest<IResult<EmployeeDetailDto>>
{
    public class Handler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetEmployeeByIdQuery, IResult<EmployeeDetailDto>>
    {
        public async Task<IResult<EmployeeDetailDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await context.DatEmployees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return await Result<EmployeeDetailDto>.FailAsync("Employee not found");

            var dto = mapper.Map<EmployeeDetailDto>(entity);

            // Xác định ngôn ngữ hiện tại
            var culture = CultureInfo.CurrentUICulture.Name;
            var isVietnamese = culture.StartsWith("vi", StringComparison.OrdinalIgnoreCase);

            // 🔹 Emergency Contact
            var contact = await context.DatEmployeeEmergencyContacts.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNo == entity.EmployeeNo && !x.IsDeleted, cancellationToken);

            if (contact != null)
            {
                dto.EmergencyContact = new EmployeeEmergencyContactDto
                {
                    NamePrimaryContact = contact.NamePrimaryContact,
                    RelationshipPrimaryContact = contact.RelationshipPrimaryContact,
                    PhoneNoPrimaryContact = contact.PhoneNoPrimaryContact,
                    NameSecondaryContact = contact.NameSecondaryContact,
                    RelationshipSecondaryContact = contact.RelationshipSecondaryContact,
                    PhoneNoSecondaryContact = contact.PhoneNoSecondaryContact
                };
            }
            // 🔹 Salary Info
            var salary = await context.DatEmployeeSalaries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNo == entity.EmployeeNo && !x.IsDeleted, cancellationToken);
            if (salary != null)
            {
                dto.SalaryInfo = new EmployeeSalaryDto
                {
                    SalaryBasic = salary.SalaryBasic,
                    SalaryGross = salary.SalaryGross,
                    BankName = salary.BankName,
                    BankNumber = salary.BankNumber,
                    BankAccountName = salary.BankAccountName,
                    PaymentType = salary.PaymentType
                };
            }

            // 🔹 Identity Info (CCCD)
            var identity = await context.DatEmployeeIdentities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNo == entity.EmployeeNo && !x.IsDeleted, cancellationToken);

            if (identity != null)
            {
                dto.IdentityInfo = new EmployeeIdentityDto
                {
                    CitizenshipCard = identity.CitizenshipCard,
                    ProvideDateCitizenshipCard = identity.ProvideDateCitizenshipCard,
                    ProvidePlaceCitizenshipCard = identity.ProvidePlaceCitizenshipCard,
                    PhotoBeforeCitizenshipCard = identity.PhotoBeforeCitizenshipCard,
                    PhotoAfterCitizenshipCard = identity.PhotoAfterCitizenshipCard
                };
            }
            // 🔹 Document Files
            var documents = await context.DatEmployeeDocuments
                .AsNoTracking()
                .Where(x => x.EmployeeNo == entity.EmployeeNo && !x.IsDeleted)
                .OrderByDescending(x => x.LastModifiedOn ?? x.CreatedOn)
                .Select(x => new EmployeeDocumentDto
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    NameFile = x.NameFile,
                    TypeFile = x.TypeFile,
                    SizeFile = x.SizeFile,
                    GoogleDriveFileUrl = x.GoogleDriveFileUrl,
                    GoogleDriveFileId = x.GoogleDriveFileId,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    LastModifiedBy = x.LastModifiedBy,
                    LastModifiedOn = x.LastModifiedOn
                })
                .ToListAsync(cancellationToken);

            dto.Documents = documents;

            // 🔹 Danh mục (load từng cái để tránh lỗi DbContext)
            if (entity.DepartmentId.HasValue)
            {
                dto.DepartmentName = await context.MstDepartments
                    .Where(d => d.Id == entity.DepartmentId.Value)
                    .Select(d => isVietnamese ? d.NameVi : d.NameEn)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (entity.PositionId.HasValue)
            {
                dto.PositionName = await context.MstPositions
                    .Where(p => p.Id == entity.PositionId.Value)
                    .Select(p => isVietnamese ? p.NameVi : p.NameEn)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (entity.ContractTypeId.HasValue)
            {
                dto.ContractTypeName = await context.KbnContractTypes
                    .Where(c => c.Id == entity.ContractTypeId.Value)
                    .Select(c => isVietnamese ? c.NameVi : c.NameEn)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (entity.EmployeeStatusId.HasValue)
            {
                dto.EmployeeStatusName = await context.KbnEmployeeStatus
                    .Where(e => e.Id == entity.EmployeeStatusId.Value)
                    .Select(e => isVietnamese ? e.NameVi : e.NameEn)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return await Result<EmployeeDetailDto>.SuccessAsync(dto);
        }
    }
}
