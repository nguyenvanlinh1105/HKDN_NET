using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.GeneralSetting;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities;

namespace NineERP.Application.Features.GeneralSettingsFeature.Commands
{
    public class UpdateGeneralSettingsCommand(GeneralSettingRequest model) : IRequest<IResult>
    {
        public GeneralSettingRequest Model { get; set; } = model;

        public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateGeneralSettingsCommand, IResult>
        {
            public async Task<IResult> Handle(UpdateGeneralSettingsCommand request, CancellationToken cancellationToken)
            {
                var setting = await context.GeneralSettings.OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);

                if (setting == null)
                {
                    setting = new GeneralSetting();
                    await context.GeneralSettings.AddAsync(setting, cancellationToken);
                }

                // Map properties (overwrite toàn bộ)
                setting.CompanyName = request.Model.CompanyName;
                setting.ShortName = request.Model.ShortName;
                setting.TaxCode = request.Model.TaxCode;
                setting.PhoneNumber = request.Model.PhoneNumber;
                setting.Address = request.Model.Address;
                setting.Email = request.Model.Email;
                setting.BankAccountNumber = request.Model.BankAccountNumber;
                setting.BankName = request.Model.BankName;
                setting.PasswordDefault = request.Model.PasswordDefault;
                setting.FiscalYearStartDay = request.Model.FiscalYearStartDay;
                setting.AnnualLeaveDays = request.Model.AnnualLeaveDays ?? 0;
                setting.ContractNumber = request.Model.ContractNumber;
                setting.ApprovedBy = request.Model.ApprovedBy;
                setting.CancelBy = request.Model.CancelBy;
                setting.AccountHolder = request.Model.AccountHolder;
                
                setting.InsuranceCompanyPercent = request.Model.InsuranceCompanyPercent;
                setting.AccidentInsuranceCompanyPercent = request.Model.AccidentInsuranceCompanyPercent;
                setting.UnionCompanyPercent = request.Model.UnionCompanyPercent;
                setting.HealthInsuranceCompanyPercent = request.Model.HealthInsuranceCompanyPercent;

                setting.InsuranceEmployeePercent = request.Model.InsuranceEmployeePercent;
                setting.AccidentInsuranceEmployeePercent = request.Model.AccidentInsuranceEmployeePercent;
                setting.UnionEmployeePercent = request.Model.UnionEmployeePercent;
                setting.HealthInsuranceEmployeePercent = request.Model.HealthInsuranceEmployeePercent;

                setting.IncomeTaxPercent = request.Model.IncomeTaxPercent;

                var result = await context.SaveChangesAsync(cancellationToken);
                return result > 0
                    ? await Result.SuccessAsync()
                    : await Result.FailAsync("Failed to update General Settings");
            }
        }
    }
}
