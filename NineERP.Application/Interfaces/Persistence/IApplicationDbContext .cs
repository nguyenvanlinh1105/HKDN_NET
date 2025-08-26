using ERP.Domain.Entities.DatTable.Employee;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NineERP.Domain.Entities;
using NineERP.Domain.Entities.Dat;
using NineERP.Domain.Entities.Identity;
using NineERP.Domain.Entities.Kbn;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Interfaces.Persistence
{
    public interface IApplicationDbContext
    {
        // Identity
        DbSet<AppUser> Users { get; }
        DbSet<AppRole> Roles { get; }
        DbSet<AppRoleClaim> RoleClaims { get; }
        DbSet<IdentityUserRole<string>> UserRoles { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<IdentityUserToken<string>> UserTokens { get; }
        DbSet<RevokedToken> RevokedTokens { get; }
        // Entities
        DbSet<EmailTemplate> EmailTemplates { get; }
        DbSet<GeneralSetting> GeneralSettings { get; }
        DbSet<EmailSetting> EmailSettings { get; }
		DbSet<IntegrationSetting> IntegrationSettings { get; }
        #region Kbn
        DbSet<KbnLeaveType> KbnLeaveTypes { get; }
        DbSet<KbnContractType> KbnContractTypes { get; }
        DbSet<KbnEmployeeStatus> KbnEmployeeStatus { get; }
        DbSet<KbnHolidayType> KbnHolidayTypes { get; }
        DbSet<KbnDeviceType> KbnDeviceTypes { get; }
        DbSet<KbnConfiguration> KbnConfigurations { get; }
        #endregion
        #region Dat
        DbSet<DatEmployee> DatEmployees { get; }
        DbSet<DatEmployeeLogTime> DatEmployeeLogTimes { get; }
        DbSet<DatEmployeeLeave> DatEmployeeLeaves { get; }
        DbSet<DatEmployeeEmergencyContact> DatEmployeeEmergencyContacts { get; }
        DbSet<DatEmployeeEducationInfo> DatEmployeeEducationInfos { get; }
        DbSet<DatEmployeeSalary> DatEmployeeSalaries { get; }
        DbSet<DatProject> DatProjects { get; }
        DbSet<DatCustomer> DatCustomers { get; }
        DbSet<DatProjectDetail> DatProjectDetails { get; }
        DbSet<DatTask> DatTasks { get; }
        DbSet<DatTaskLog> DatTaskLogs { get; }
        DbSet<DatTaskLogTime> DatTaskLogTimes { get; }
        DbSet<DatCommentTask> DatCommentTasks { get; }
        DbSet<DatDocumentTask> DatDocumentTasks { get; }
        DbSet<DatCalendarHoliday> DatCalendarHolidays { get; }
        DbSet<DatEmployeeShift> DatEmployeeShifts { get; }
        DbSet<DatEmployeeAnnualLeave> DatEmployeeAnnualLeaves { get; }
        DbSet<DatEmployeeApprove> DatEmployeeApproves { get; }
        DbSet<DatLeaveApprove> DatLeaveApproves { get; }
        DbSet<DatEmployeeLeaveDetail> DatEmployeeLeaveDetails { get; }
        DbSet<DatEmployeeIdentity> DatEmployeeIdentities { get; }
        DbSet<DatEmployeeDocument> DatEmployeeDocuments { get; }
        DbSet<DatEmployeeDocumentLog> DatEmployeeDocumentLogs { get; }

        #endregion
        #region Mst
        DbSet<MstDepartment> MstDepartments { get; }
        DbSet<MstPosition> MstPositions { get; }
        DbSet<MstTeam> MstTeams { get; }
        DbSet<MstStatusTask> MstStatusTasks { get; }
        DbSet<MstPriorityTask> MstPriorityTasks { get; }
        DbSet<MstTypeTask> MstTypeTasks { get; }
        DbSet<MstActivity> MstActivities { get; }
        DbSet<MstProgrammingLanguage> MstProgrammingLanguages { get; }
        DbSet<MstShift> MstShifts { get; }
        DbSet<MstCountry> MstCountry { get; }
        DbSet<MstProvinces> MstProvinces { get; }
        #endregion
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
