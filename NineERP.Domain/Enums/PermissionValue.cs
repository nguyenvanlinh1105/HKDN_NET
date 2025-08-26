namespace NineERP.Domain.Enums
{
    public class PermissionValue
    {
        public const string PermissionType = "permission";

        public class Dashboard
        {
            public const string View = "DashboardView";
        }
        public class Employees
        {
            public const string View = "EmployeesView";
            public const string Update = "EmployeesUpdate";
            public const string Add = "EmployeesAdd";
            public const string Delete = "EmployeesDelete";
        }
        public class Customers
        {
            public const string View = "CustomersView";
            public const string Update = "CustomersUpdate";
            public const string Add = "CustomersAdd";
            public const string Delete = "CustomersDelete";
        }
        public class Departments
        {
            public const string View = "DepartmentsView";
            public const string Update = "DepartmentsUpdate";
            public const string Add = "DepartmentsAdd";
            public const string Delete = "DepartmentsDelete";
        }
        public class KbnContractTypes 
        {
            public const string View = "KbnContractTypesView";
            public const string Update = "KbnContractTypesUpdate";
            public const string Add = "KbnContractTypesAdd";
            public const string Delete = "KbnContractTypesDelete";
        }
        public class KbnEmployeeStatus
        {
            public const string View = "KbnEmployeeStatusView";
            public const string Update = "KbnEmployeeStatusUpdate";
            public const string Add = "KbnEmployeeStatusAdd";
            public const string Delete = "KbnEmployeeStatusDelete";
        }
        public class KbnLeaveTypes
        {
            public const string View = "KbnLeaveTypesView";
            public const string Update = "KbnLeaveTypesUpdate";
            public const string Add = "KbnLeaveTypesAdd";
            public const string Delete = "KbnLeaveTypesDelete";
        }
        public class KbnHolidayTypes
        {
            public const string View = "KbnHolidayTypesView";
            public const string Update = "KbnHolidayTypesUpdate";
            public const string Add = "KbnHolidayTypesAdd";
            public const string Delete = "KbnHolidayTypesDelete";
        }
        public class KbnDeviceTypes
        {
            public const string View = "KbnDeviceTypesView";
            public const string Update = "KbnDeviceTypesUpdate";
            public const string Add = "KbnDeviceTypesAdd";
            public const string Delete = "KbnDeviceTypesDelete";
        }
        public class MstProgrammingLanguages
        {
            public const string View = "MstProgrammingLanguagesView";
            public const string Update = "MstProgrammingLanguagesUpdate";
            public const string Add = "MstProgrammingLanguagesAdd";
            public const string Delete = "MstProgrammingLanguagesDelete";
        }
        public class MstTeams
        {
            public const string View = "MstTeamsView";
            public const string Update = "MstTeamsUpdate";
            public const string Add = "MstTeamsAdd";
            public const string Delete = "MstTeamsDelete";
        }
        public class MstShifts
        {
            public const string View = "MstShiftsView";
            public const string Update = "MstShiftsUpdate";
            public const string Add = "MstShiftsAdd";
            public const string Delete = "MstShiftsDelete";
        }
        public class Report
        {
            public const string View = "ReportView";
            public const string Update = "ReportUpdate";
            public const string Add = "ReportAdd";
            public const string Delete = "ReportDelete";
        }
        public class System
        {
            public const string View = "SystemView";
        }

        public class Users
        {
            public const string View = "UserView";
            public const string Update = "UserUpdate";
            public const string Add = "UserAdd";
            public const string Delete = "UserDelete";
        }

        public class Roles
        {
            public const string View = "RolesView";
            public const string Update = "RolesUpdate";
            public const string Add = "RolesAdd";
            public const string Delete = "RolesDelete";
        }
        public class AuditLogs
        {
            public const string View = "AuditLogsView";
            public const string Export = "AuditLogsExport";
        }
        public class GeneralSettings
        {
            public const string View = "GeneralSettingsView";
            public const string Update = "GeneralSettingsUpdate";
        }
        public class EmailSettings
        {
            public const string View = "EmailSettingsView";
            public const string Update = "EmailSettingsUpdate";
        }
        public class EmailTemplates
        {
            public const string View = "EmailTemplatesView";
            public const string Update = "EmailTemplatesUpdate";
            public const string Add = "EmailTemplatesAdd";
            public const string Delete = "EmailTemplatesDelete";
        }
        public class IntegrationSettings
        {
            public const string View = "IntegrationSettingsView";
            public const string Update = "IntegrationSettingsUpdate";
            public const string TestUpload = "IntegrationSettingsTestUpload";
        }
    }
}
