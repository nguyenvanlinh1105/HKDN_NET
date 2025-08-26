using AutoMapper;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Constants.Role;
using NineERP.Application.Constants.User;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;
using NineERP.Domain.Entities.Identity;
using NineERP.Domain.Entities.Mst;
using NineERP.Domain.Entities;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class CreateEmployeeCommand : IRequest<IResult>
{
    public EmployeeDetailDto Employee { get; set; } = new();
    public string Origin { get; set; } = default!;
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _mailService;

    public CreateEmployeeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        UserManager<AppUser> userManager,
        IEmailService mailService)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _mailService = mailService;
    }

    public async Task<IResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        try
        {
            var dto = request.Employee;

            // 1. Map và tạo DatEmployee
            var employee = _mapper.Map<DatEmployee>(dto);
            employee.CreatedOn = DateTime.Now;
            _context.DatEmployees.Add(employee);

            // 2. Tạo AppUser
            var user = new AppUser
            {
                Email = dto.Email,
                FullName = dto.FullName,
                UserName = dto.EmployeeNo,
                PhoneNumber = dto.PhoneNo,
                EmailConfirmed = true,
                CreatedBy = dto.EmployeeNo,
                AvatarUrl = dto.ImageURL,
                CreatedOn = DateTime.Now,
                IsDeleted = false
            };

            if (await _userManager.FindByEmailAsync(user.Email) is not null)
                return await Result.FailAsync("Email already exists");

            var result = await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
            if (!result.Succeeded)
                return await Result.FailAsync("Failed to create user");

            await _userManager.AddToRoleAsync(user, RoleConstants.Employee);

            // 3. Gán ca làm việc mặc định
            var defaultShift = await _context.MstShifts.FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);
            if (defaultShift != null)
            {
                _context.DatEmployeeShifts.Add(new DatEmployeeShift
                {
                    EmployeeNo = dto.EmployeeNo,
                    ShiftId = defaultShift.Id
                });
            }

            // 4. Gán người duyệt và hủy từ GeneralSetting
            var setting = await _context.GeneralSettings.FirstOrDefaultAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(setting?.ApprovedBy))
            {
                _context.DatEmployeeApproves.Add(new DatEmployeeApprove
                {
                    EmployeeNoLeave = dto.EmployeeNo,
                    EmployeeNoApprove = setting.ApprovedBy,
                    ApproveLevel = 1,
                    ApproveType = (short)StaticVariable.ApproveType.Approve,
                    IsDeleted = false
                });
            }


            if (!string.IsNullOrWhiteSpace(setting?.CancelBy))
            {
                _context.DatEmployeeApproves.Add(new DatEmployeeApprove
                {
                    EmployeeNoLeave = dto.EmployeeNo,
                    EmployeeNoApprove = setting.CancelBy,
                    ApproveLevel = 0,
                    ApproveType = (short)StaticVariable.ApproveType.Cancel,
                    IsDeleted = false
                });
            }

            // 5. Tính ngày phép năm
            var status = await _context.KbnEmployeeStatus.FirstOrDefaultAsync(x => x.Id == dto.EmployeeStatusId, cancellationToken);
            var leaveDays = 0.0m;
            if (status != null && status.IsOfficial && dto.WorkingDateFrom.HasValue)
            {
                var wd = dto.WorkingDateFrom.Value;
                var year = DateTime.Now.Month < 4 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
                var fyEnd = new DateTime(year + 1, 3, 31);
                var diffMonth = (fyEnd.Year - wd.Year) * 12 + fyEnd.Month - wd.Month;
                leaveDays = diffMonth >= 0 && diffMonth <= 12 ? diffMonth + 1 : 0;
                if (leaveDays > 0 && wd.Day > 15) leaveDays--;
            }

            _context.DatEmployeeAnnualLeaves.Add(new DatEmployeeAnnualLeave
            {
                EmployeeNo = dto.EmployeeNo,
                LeaveCurrentYear = leaveDays,
                LeaveLastYear = 0,
                LeaveUsed = 0
            });

            // 6. Tăng số mã nhân viên
            var empNoConfig = await _context.KbnConfigurations.FirstOrDefaultAsync(x => x.Code == EmployeeNoConstants.EmployeeNumberKey, cancellationToken);
            if (empNoConfig != null) empNoConfig.IntValue++;

            // 7. Tăng mã số hợp đồng
            if (dto.ContractTypeId.HasValue)
            {
                var contractType = await _context.KbnContractTypes
                    .FirstOrDefaultAsync(x => x.Id == dto.ContractTypeId.Value, cancellationToken);

                if (contractType != null &&
                    ContractNoConstants.GroupCodeToNumberKeyMap.TryGetValue(contractType.GroupCode.ToUpper(), out var numberKey))
                {
                    var contractConfig = await _context.KbnConfigurations
                        .FirstOrDefaultAsync(x => x.Code == numberKey, cancellationToken);

                    if (contractConfig != null)
                    {
                        contractConfig.IntValue++;
                    }
                }
            }

            // 8. Gửi email
            var template = await _context.EmailTemplates.FirstOrDefaultAsync(x =>
                x.Code == "CONFIRM_EMAIL" && x.IsActive && x.Language == "vi", cancellationToken);

            if (template == null)
                return await Result.FailAsync("Email template not found");

            var subject = template.Subject;
            var body = template.Body
                .Replace("{{FullName}}", user.FullName)
                .Replace("{{Email}}", user.Email)
                .Replace("{{Password}}", UserConstants.DefaultPassword)
                .Replace("{{LoginLink}}", $"{request.Origin}/login");

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            BackgroundJob.Enqueue(() => _mailService.SendAsync(user.Email, subject, body));

            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return await Result.FailAsync($"Create failed: {ex.Message}");
        }
    }
}
