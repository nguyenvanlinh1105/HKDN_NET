using ERP.Domain.Entities.DatTable.Employee;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Entities;
using NineERP.Domain.Entities.Base;
using NineERP.Domain.Entities.Dat;
using NineERP.Domain.Entities.Identity;
using NineERP.Domain.Entities.Kbn;
using NineERP.Domain.Entities.Mst;
using NineERP.Infrastructure.Helpers;

namespace NineERP.Infrastructure.Contexts
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : IdentityDbContext<AppUser, AppRole, string,
            IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>(options),
          IApplicationDbContext
    {
        #region DbSets

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<CronJob> CronJobs => Set<CronJob>();
        public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
        public DbSet<GeneralSetting> GeneralSettings => Set<GeneralSetting>();
        public DbSet<EmailSetting> EmailSettings => Set<EmailSetting>();
		public DbSet<IntegrationSetting> IntegrationSettings => Set<IntegrationSetting>();
        public new DbSet<IdentityUserToken<string>> UserTokens => Set<IdentityUserToken<string>>();
        public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

        #region Kbn
        public DbSet<KbnLeaveType> KbnLeaveTypes => Set<KbnLeaveType>();
        public DbSet<KbnContractType> KbnContractTypes => Set<KbnContractType>();
        public DbSet<KbnEmployeeStatus> KbnEmployeeStatus => Set<KbnEmployeeStatus>();
        public DbSet<KbnHolidayType> KbnHolidayTypes => Set<KbnHolidayType>();
        public DbSet<KbnDeviceType> KbnDeviceTypes => Set<KbnDeviceType>();
        public DbSet<KbnConfiguration> KbnConfigurations => Set<KbnConfiguration>();
        #endregion

        #region Dat
        public DbSet<DatEmployeeLogTime> DatEmployeeLogTimes => Set<DatEmployeeLogTime>();
        public DbSet<DatEmployeeLeave> DatEmployeeLeaves => Set<DatEmployeeLeave>();
        public DbSet<DatEmployee> DatEmployees => Set<DatEmployee>();
        public DbSet<DatEmployeeEmergencyContact> DatEmployeeEmergencyContacts => Set<DatEmployeeEmergencyContact>();
        public DbSet<DatEmployeeEducationInfo> DatEmployeeEducationInfos => Set<DatEmployeeEducationInfo>();
        public DbSet<DatEmployeeSalary> DatEmployeeSalaries => Set<DatEmployeeSalary>();
        public DbSet<DatProject> DatProjects => Set<DatProject>();
        public DbSet<DatProjectDetail> DatProjectDetails => Set<DatProjectDetail>();
        public DbSet<DatCustomer> DatCustomers => Set<DatCustomer>();
        public DbSet<DatTask> DatTasks => Set<DatTask>();
        public DbSet<DatTaskLog> DatTaskLogs => Set<DatTaskLog>();
        public DbSet<DatTaskLogTime> DatTaskLogTimes => Set<DatTaskLogTime>();
        public DbSet<DatCommentTask> DatCommentTasks => Set<DatCommentTask>();
        public DbSet<DatDocumentTask> DatDocumentTasks => Set<DatDocumentTask>();
        public DbSet<DatCalendarHoliday> DatCalendarHolidays => Set<DatCalendarHoliday>();
        public DbSet<DatEmployeeShift> DatEmployeeShifts => Set<DatEmployeeShift>();
        public DbSet<DatEmployeeAnnualLeave> DatEmployeeAnnualLeaves => Set<DatEmployeeAnnualLeave>();
        public DbSet<DatEmployeeApprove> DatEmployeeApproves => Set<DatEmployeeApprove>();
        public DbSet<DatLeaveApprove> DatLeaveApproves => Set<DatLeaveApprove>();
        public DbSet<DatEmployeeLeaveDetail> DatEmployeeLeaveDetails => Set<DatEmployeeLeaveDetail>();
        public DbSet<DatEmployeeIdentity> DatEmployeeIdentities => Set<DatEmployeeIdentity>();
        public DbSet<DatEmployeeDocument> DatEmployeeDocuments => Set<DatEmployeeDocument>();
        public DbSet<DatEmployeeDocumentLog> DatEmployeeDocumentLogs => Set<DatEmployeeDocumentLog>();

        #endregion

        #region Mst
        public DbSet<MstDepartment> MstDepartments => Set<MstDepartment>();
        public DbSet<MstPosition> MstPositions => Set<MstPosition>();
        public DbSet<MstTeam> MstTeams => Set<MstTeam>();
        public DbSet<MstStatusTask> MstStatusTasks => Set<MstStatusTask>();
        public DbSet<MstPriorityTask> MstPriorityTasks => Set<MstPriorityTask>();
        public DbSet<MstTypeTask> MstTypeTasks => Set<MstTypeTask>();
        public DbSet<MstActivity> MstActivities => Set<MstActivity>();
        public DbSet<MstProgrammingLanguage> MstProgrammingLanguages => Set<MstProgrammingLanguage>();
        public DbSet<MstShift> MstShifts => Set<MstShift>();
        public DbSet<MstCountry> MstCountry => Set<MstCountry>();
        public DbSet<MstProvinces> MstProvinces => Set<MstProvinces>();

        #endregion

        #endregion

        #region SaveChanges + Audit
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Handle audit timestamps
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = dateTimeService.Now;
                        entry.Entity.CreatedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = dateTimeService.Now;
                        entry.Entity.LastModifiedBy = currentUserService.UserName ?? "SuperAdmin";
                        break;
                }
            }

            // Build Audit Logs
            var auditLogs = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            e.State != EntityState.Detached &&
                            e.State != EntityState.Unchanged))
            {
                var audit = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    ActionType = entry.State.ToString()
                };

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        audit.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            audit.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            audit.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                audit.OldValues[propertyName] = property.OriginalValue;
                                audit.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                auditLogs.Add(audit.ToAuditLog(
                    currentUserService.UserId ?? "SYSTEM",
                    currentUserService.UserName ?? "SYSTEM",
                    currentUserService.Origin
                ));
            }

            if (auditLogs.Any())
                AuditLogs.AddRange(auditLogs);

            return await base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Identity Model Config

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("EmailIndex");
            });

            builder.Entity<AppRole>().ToTable("Roles", "Identity");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Identity");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Identity");

            builder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims", "Identity");
                entity.HasOne(rc => rc.Role)
                      .WithMany(r => r.RoleClaims)
                      .HasForeignKey(rc => rc.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        #endregion
    }
}
