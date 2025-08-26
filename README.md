# 🧾 NineERP - Enterprise Resource Planning System

NineERP is a modular, multilingual, and secure ERP system tailored for SMEs and enterprises. Built on modern .NET technology, it integrates identity, multi-role permission, background jobs, notifications, and more.

---

## 🚀 Features

- Modular architecture following Clean Architecture
- Multi-language support (Vietnamese, English)
- Role-based access control with dynamic permissions
- User management, audit logging, and login history
- Email service via SMTP (MailKit)
- Background jobs with Hangfire (queued email, scheduled tasks)
- Logging using Serilog
- Integrated reCAPTCHA

---

## 🛠 Technologies Used

| Area         | Stack                                   |
|--------------|-----------------------------------------|
| Backend      | .NET 9, ASP.NET Core MVC|
| Identity     | ASP.NET Identity + Cookie Auth          |
| UI Template  | Minton Bootstrap Admin Template         |
| Background   | Hangfire + SQL Server storage           |
| ORM          | Entity Framework Core                   |
| Email        | MailKit via SMTP                        |
| Validation   | FluentValidation                        |
| Logging      | Serilog                                 |
| Localization | .resx Resources + IViewLocalizer        |

---

## ⚙️ Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/your-org/nineerp.git
cd nineerp
```

### 2. Configuration

#### `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=NineERP;Trusted_Connection=True;"
},
"MailConfiguration": {
  "From": "noreply@nineerp.com",
  "DisplayName": "NineERP",
  "Host": "smtp.gmail.com",
  "Port": 587,
  "UserName": "your-email",
  "Password": "your-app-password"
},
"Session": {
  "IdleTimeoutHours": 4,
  "DefaultLockoutTimeSpan": 10
}
```

> Also setup `Resources` folder for localization and email templates in DB.

### 3. Apply Database Migration
```bash
dotnet ef database update --project NineERP.Infrastructure
dotnet ef migrations add ChangDepartmentTable --project /Users/duongphuc/Project/NineERP/NineERP.Infrastructure --startup-project /Users/duongphuc/Project/NineERP/NineERP.Web

dotnet ef database update --project /Users/duongphuc/Project/NineERP/NineERP.Infrastructure --startup-project /Users/duongphuc/Project/NineERP/NineERP.Web

```
> Ensure SQL Server is running and connection string is correct.

### 4. Run the Application
```bash
dotnet run --project NineERP.Web
```

Then open browser at: `http://localhost:5000`

### 5. Access Hangfire Dashboard
- URL: `http://localhost:5000/hangfire`
- Requires login with role `Admin` or `SuperAdmin`

---

## 🧩 Project Structure

```
NineERP
├── NineERP.Domain            # Entities, Enums, Interfaces
├── NineERP.Application       # DTOs, Services, CQRS Features
├── NineERP.Infrastructure    # EF Core, Repositories, Services
├── NineERP.Web               # Razor UI, Controllers, Views
└── README.md
```

---

## 🧪 Seed Users and Roles
The app seeds default roles and an admin user on startup:

- **Username:** admin
- **Password:** NineErp@123

Check `DatabaseSeeder.cs` to customize seeding logic.

---

## 📦 Deployment Notes

- Publish with: `dotnet publish -c Release`
- Deploy to IIS, Linux with Nginx, or containers
- Use `appsettings.Production.json` and environment variables
- SMTP should be secured and tested per environment

---

## 📚 Documentation

- [Feature: Forgot Password](docs/ForgotPassword.md)
- [Feature: Audit Logging](docs/AuditLog.md)
- [Feature: Role Permission System](docs/Permission.md)
- [API Extensions](docs/API.md) *(optional)*

---

## 🧠 Contributing

Pull requests and feature suggestions are welcome. Please follow the code style and use commit conventions.

---

## 👨‍💻 Author
Developed and maintained by **NinePlus Solution., JSC**

Website: [https://nineplus.com.vn](https://nineplus.com.vn)

---

## 📄 License
MIT License - free for commercial and personal use.

