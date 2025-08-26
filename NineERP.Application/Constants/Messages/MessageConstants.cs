namespace NineERP.Application.Constants.Messages
{
    public static class MessageConstants
    {
        public const string NotFound = "Không tìm thấy dữ liệu.";
        public const string UpdateSuccess = "Cập nhật thành công.";
        public const string UpdateFail = "Cập nhật thất bại.";
        public const string AddSuccess = "Tạo thành công.";
        public const string AddFail = "Tạo không thành công.";
        public const string DeleteSuccess = "Xóa thành công.";
        public const string DeleteFail = "Xóa không thành công.";
        public const string ValidationPassword = "Password must contain at least 9 characters, including at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&)!";
    }

    public static class ErrorMessages
    {
        public static readonly Dictionary<string, string> Errors = new()
        {
            // System
            { "SYS0001", "Update successful." },
            { "SYS0002", "Update failed." },
            { "SYS0003", "Create success." },
            { "SYS0004", "Create failed." },
            { "SYS0006", "Delete success." },
            { "SYS0005", "Delete failed." },
            // Auth
            { "AUTH000", "Invalid username or password." },
            { "AUTH001", "Username or password is incorrect." },
            { "AUTH002", "Account locked due to multiple failed login attempts." },
            { "AUTH003", "Your account is locked." },
            { "AUTH004", "Token expired. Please login again." },
            { "AUTH005", "Unauthorized" },
            { "AUTH006", "Resource not found" },
            { "AUTH007", "Internal server error" },
            { "AUTH008", "You do not have permission to access this resource." },
            { "AUTH009", "Invalid refresh token." },
            { "AUTH010", "Refresh token expired." },
            { "AUTH011", "Logged out Fail." },
            // Forgot Password
            { "FP001", "Forgot Password failed." },

            { "DATA001", "Data does not exist." },
            //EmployeeLogTime
            { "ELT001", "You have already checked in today." },
            { "ELT002", "You have not checked in today. Please check in first." },
            { "ELT003", "You have checked out today." },
            { "ELT004", "Invalid type provided." },
            { "ELT005", "You cannot edit because this record has been previously edited.." },
            { "ELT006", "You have used up all your edits for the month." },
            //Employee
            { "E001", "Invalid Employee." },
            { "E002", "Employee already exists." },
            { "E003", "Employee not found." },
            { "E004", "Invalid Employee ID." },
            { "E005", "Invalid Employee No." },
            { "E006", "Invalid Employee Full Name." },
            { "E007", "Invalid Employee Email." },
            { "E008", "Invalid Employee Phone." },
            { "E009", "Invalid Employee Address." },
            { "E010", "Invalid Employee Department." },
            { "E011", "Invalid Employee Position." },
            { "E012", "Invalid Employee Status." },
            { "E013", "Employee number is required." },
            { "E014", "Employee number must not exceed 50 characters." },
            { "E015", "Full name is required." },
            { "E016", "Full name must not exceed 255 characters." },
            { "E017", "Nickname is required." },
            { "E018", "Nickname must not exceed 255 characters." },
            { "E019", "Email is required." },
            { "E020", "Invalid email format." },
            { "E021", "Email must not exceed 255 characters." },
            { "E022", "Phone number must not exceed 15 digits." },
            { "E023", "Invalid phone number format." },
            { "E024", "Birthday must be in the past." },
            { "E025", "Gender must be 0 (Male), 1 (Female), or 2 (Other)." },
            { "E026", "Invalid marital status." },
            { "E027", "Number of children cannot be negative." },
            { "E028", "Identity number must not exceed 255 characters." },
            { "E029", "Provide date must be today or in the past." },
            { "E030", "Provide place must not exceed 255 characters." },
            { "E031", "Place of birth must not exceed 255 characters." },
            { "E032", "Address must not exceed 255 characters." },
            { "E033", "Email already exists." },
            // Emergency Contact
            { "EC001", "Primary contact name is required." },
            { "EC002", "Primary contact name must not exceed 255 characters." },
            { "EC003", "Primary contact phone number is required." },
            { "EC004", "Primary contact phone number must not exceed 15 digits." },
            { "EC005", "Invalid primary contact phone number format." },
            { "EC006", "Secondary contact name is required." },
            { "EC007", "Secondary contact name must not exceed 255 characters." },
            { "EC008", "Secondary contact phone number is required." },
            { "EC009", "Secondary contact phone number must not exceed 15 digits." },
            { "EC010", "Invalid secondary contact phone number format." },
            { "EC011", "Primary contact relationship is required." },
            { "EC012", "Primary contact relationship must not exceed 50 characters." },
            { "EC013", "Secondary contact relationship is required." },
            { "EC014", "Secondary contact relationship must not exceed 50 characters." },
            { "EC015", "Invalid Emergency Contact." }, 
            // Education Info
            { "EI001", "Education is required." },
            { "EI002", "Education must not exceed 255 characters." },
            { "EI003", "Business experience is required." },
            { "EI004", "Business experience must not exceed 255 characters." },
            { "EI005", "Soft skills are required." },
            { "EI006", "Soft skills must not exceed 255 characters." },
            { "EI007", "English level is required." },
            { "EI008", "English level must not exceed 255 characters." },
            { "EI009", "Japanese level is required." },
            { "EI010", "Japanese level must not exceed 255 characters." },
            { "EI011", "Invalid Education Info." },
            // Back and Salary
            { "BS001", "Back and Salary Info is required." },
            { "BS002", "Back and Salary Info must not exceed 255 characters." },
            { "BS003", "Invalid Back and Salary Info." },
            { "BA001", "Bank name must not exceed 255 characters." },
            { "BA002", "Bank number must not exceed 30 characters." },
            { "BA003", "Bank account name must not exceed 255 characters." },
            { "BA004", "Invalid payment type." },
            { "BA005", "Bank name is required for bank transfer." },
            { "BA006", "Bank number is required for bank transfer." },
            { "BA007", "Bank account name is required for bank transfer." },
            // Position General Info
            { "PGI001", "Invalid position general info." },
            // Project
            { "P001", "Project does not exist." },
            { "P002", "Project already exists." },
            { "P003", "Project not found." },
            { "P004", "Invalid project ID." },
            { "P005", "Invalid project name." },
            { "P006", "Invalid project type." },
            { "P007", "Invalid project status." },
            { "P008", "Invalid project duration." },
            { "P009", "Invalid project start date." },
            { "P010", "Invalid project end date." },
            { "P011", "Invalid customer name." },
            { "P012", "Invalid technology." },
            { "P013", "Invalid note." },
            { "P014", "Invalid image URL." },
            // Task
            { "T001", "Task does not exist." },
            // Comment Task
            { "TC001", "Invalid task." },
            { "TC002", "Comment must not be empty." },
            { "TC003", "Comment must not exceed 1000 characters." },
            { "TC004", "Comment does not exist." },
            { "TC005", "Comments are not yours so you have no right to delete them." },
        };

        public static string GetMessage(string code) => Errors.GetValueOrDefault(code, "Unknown error.");
    }
}
