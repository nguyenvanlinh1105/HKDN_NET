using MediatR;
using NineERP.Application.Dtos.GeneralSetting;
using NineERP.Application.Wrapper;
using NineERP.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NineERP.Application.Features.GeneralSettingsFeature.Queries
{
    public record GetGeneralSettingsQuery() : IRequest<Result<GeneralSettingDto>>
    {
        public class Handler(IApplicationDbContext context) : IRequestHandler<GetGeneralSettingsQuery, Result<GeneralSettingDto>>
        {
            public async Task<Result<GeneralSettingDto>> Handle(GetGeneralSettingsQuery request, CancellationToken cancellationToken)
            {
                var setting = await context.GeneralSettings.FirstOrDefaultAsync(cancellationToken);

                if (setting == null)
                {
                    return await Result<GeneralSettingDto>.FailAsync("No General Settings found.");
                }

                var result = new GeneralSettingDto
                {
                    CompanyName = setting.CompanyName,
                    ShortName = setting.ShortName,
                    TaxCode = setting.TaxCode,
                    PhoneNumber = setting.PhoneNumber,
                    Address = setting.Address,
                    Email = setting.Email,
                    BankAccountNumber = setting.BankAccountNumber,
                    BankName = setting.BankName,
                    PasswordDefault = setting.PasswordDefault,
                    FiscalYearStartDay = setting.FiscalYearStartDay,
                    AnnualLeaveDays = setting.AnnualLeaveDays,
                    ContractNumber = setting.ContractNumber,
                    ApprovedBy = setting.ApprovedBy,
                    CancelBy = setting.CancelBy,
                    AccountHolder = setting.AccountHolder,
                    InsuranceCompanyPercent = setting.InsuranceCompanyPercent,
                    HealthInsuranceCompanyPercent = setting.HealthInsuranceCompanyPercent,
                    AccidentInsuranceCompanyPercent = setting.AccidentInsuranceCompanyPercent,
                    UnionCompanyPercent = setting.UnionCompanyPercent,
                    InsuranceEmployeePercent = setting.InsuranceEmployeePercent,
                    HealthInsuranceEmployeePercent = setting.HealthInsuranceEmployeePercent,
                    AccidentInsuranceEmployeePercent = setting.AccidentInsuranceEmployeePercent,
                    UnionEmployeePercent = setting.UnionEmployeePercent,
                    IncomeTaxPercent = setting.IncomeTaxPercent
                };

                return await Result<GeneralSettingDto>.SuccessAsync(result);
            }
        }
    }
}
