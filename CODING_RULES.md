# NineERP 2025 - Coding Rules & Standards

## 📋 Tổng quan
Tài liệu này định nghĩa các quy tắc và tiêu chuẩn coding cho dự án NineERP 2025, một hệ thống ERP được xây dựng trên .NET 9.0 với kiến trúc Clean Architecture.

## 🏗️ Kiến trúc dự án

### Cấu trúc thư mục
```
NineERP_2025/
├── NineERP.Domain/           # Domain Layer - Entities, Interfaces, Enums
├── NineERP.Application/       # Application Layer - DTOs, Commands, Queries, Validators
├── NineERP.Infrastructure/   # Infrastructure Layer - DbContext, Services, Migrations
└── NineERP.Web/             # Presentation Layer - Controllers, Views, Middleware
```

### Nguyên tắc Clean Architecture
- **Domain Layer**: Chứa business logic và entities, không phụ thuộc vào bất kỳ layer nào khác
- **Application Layer**: Chứa business use cases, DTOs, và interfaces
- **Infrastructure Layer**: Implement các interfaces từ Application layer
- **Web Layer**: Xử lý HTTP requests và responses

## 🎯 Quy tắc chung

### 1. Naming Conventions

#### Namespaces
```csharp
// ✅ Đúng
namespace NineERP.Application.Features.EmployeesFeature.Commands
namespace NineERP.Domain.Entities.Dat
namespace NineERP.Infrastructure.Services.Common

// ❌ Sai
namespace NineERP.Application.employees.commands
namespace NineERP.Domain.entities.dat
```

#### Classes
```csharp
// ✅ Đúng - PascalCase
public class CreateEmployeeCommand : IRequest<IResult>
public class EmployeeDetailDto
public class ValidationBehavior<TRequest, TResponse>

// ❌ Sai
public class createEmployeeCommand
public class employeeDetailDto
```

#### Methods
```csharp
// ✅ Đúng - PascalCase
public async Task<IResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
public static IResult Success(string message)
public bool HasPermission(string permission)

// ❌ Sai
public async Task<IResult> handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
public static IResult success(string message)
```

#### Properties
```csharp
// ✅ Đúng - PascalCase
public string FullName { get; set; }
public DateTime CreatedOn { get; set; }
public bool IsDeleted { get; set; }

// ❌ Sai
public string fullName { get; set; }
public DateTime createdOn { get; set; }
```

#### Constants
```csharp
// ✅ Đúng - PascalCase
public const string NotFound = "Không tìm thấy dữ liệu.";
public const string UpdateSuccess = "Cập nhật thành công.";
public static readonly Dictionary<string, string> Errors = new();

// ❌ Sai
public const string notFound = "Không tìm thấy dữ liệu.";
public const string UPDATE_SUCCESS = "Cập nhật thành công.";
```

### 2. File Organization

#### Mỗi file chỉ chứa một class/interface
```csharp
// ✅ Đúng - Mỗi file một class
// CreateEmployeeCommand.cs
public class CreateEmployeeCommand : IRequest<IResult>
{
    // Implementation
}

// CreateEmployeeCommandHandler.cs  
public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResult>
{
    // Implementation
}
```

#### Tên file phải khớp với tên class
```csharp
// ✅ Đúng
// File: CreateEmployeeCommand.cs
public class CreateEmployeeCommand

// File: EmployeeDetailDto.cs
public class EmployeeDetailDto

// ❌ Sai
// File: EmployeeCommand.cs
public class CreateEmployeeCommand
```

### 3. Code Structure

#### Using statements
```csharp
// ✅ Đúng - Sắp xếp theo thứ tự
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;
using NineERP.Domain.Entities.Identity;
```

#### Constructor injection
```csharp
// ✅ Đúng - Sử dụng primary constructor (C# 12)
public class CreateEmployeeCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    UserManager<AppUser> userManager,
    IEmailService mailService) : IRequestHandler<CreateEmployeeCommand, IResult>

// ✅ Đúng - Sử dụng traditional constructor
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
}
```

## 🔧 Application Layer Rules

### 1. DTOs (Data Transfer Objects)

#### Naming
```csharp
// ✅ Đúng
public class EmployeeDetailDto
public class EmployeeGeneralDto
public class CreateEmployeeRequest
public class UpdateEmployeeResponse

// ❌ Sai
public class EmployeeDTO
public class EmployeeGeneral
public class CreateEmployee
```

#### Structure
```csharp
// ✅ Đúng
public class EmployeeDetailDto
{
    public string EmployeeNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public DateTime? Birthday { get; set; }
    public short? Gender { get; set; }
    public short? MaritalStatus { get; set; }
    public int? NumberOfChildren { get; set; }
    public string? ImageURL { get; set; }
    public string? Address { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? EmployeeStatusId { get; set; }
    public int? ContractTypeId { get; set; }
    public DateTime? WorkingDateFrom { get; set; }
    public DateTime? WorkingDateTo { get; set; }
    public string? Note { get; set; }
}
```

### 2. Commands & Queries (CQRS Pattern)

#### Naming
```csharp
// ✅ Đúng
public class CreateEmployeeCommand : IRequest<IResult>
public class UpdateEmployeeCommand : IRequest<IResult>
public class DeleteEmployeeCommand : IRequest<IResult>
public class GetEmployeeByIdQuery : IRequest<IResult<EmployeeDetailDto>>
public class GetAllEmployeesQuery : IRequest<IResult<List<EmployeeGeneralDto>>>

// ❌ Sai
public class CreateEmployee
public class UpdateEmployee
public class EmployeeQuery
```

#### Handler Implementation
```csharp
// ✅ Đúng
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
        // Implementation
    }
}
```

### 3. Validation

#### Sử dụng FluentValidation
```csharp
// ✅ Đúng
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Employee.FullName)
            .NotEmpty().WithMessage("Full name must not be empty.");

        RuleFor(x => x.Employee.Email)
            .NotEmpty().WithMessage("Email must not be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Employee.EmployeeNo)
            .NotEmpty().WithMessage("Employee number must not be empty.");

        RuleFor(x => (int)(x.Employee.PositionId ?? 0))
            .GreaterThan(0).WithMessage("Invalid position.");

        RuleFor(x => (int)(x.Employee.DepartmentId ?? 0))
            .GreaterThan(0).WithMessage("Invalid department.");
    }
}
```

### 4. Response Wrapper

#### Sử dụng IResult pattern
```csharp
// ✅ Đúng
public interface IResult
{
    List<string> Messages { get; set; }
    bool Succeeded { get; set; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}

// Sử dụng trong handlers
return await Result.SuccessAsync(MessageConstants.AddSuccess);
return await Result.FailAsync("Email already exists");
return await Result.SuccessAsync(data, "Operation successful");
```

## 🏛️ Domain Layer Rules

### 1. Entities

#### Base Entity Structure
```csharp
// ✅ Đúng
public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{
}

public interface IAuditableEntity : IEntity
{
    string CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    string? LastModifiedBy { get; set; }
    DateTime? LastModifiedOn { get; set; }
    bool IsDeleted { get; set; }
}

public class AuditableBaseEntity<TId> : IAuditableEntity<TId>
{
    public TId Id { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```

#### Entity Naming
```csharp
// ✅ Đúng
public class DatEmployee : AuditableBaseEntity<string>
public class MstDepartment : AuditableBaseEntity<int>
public class KbnEmployeeStatus : AuditableBaseEntity<int>

// ❌ Sai
public class Employee : AuditableBaseEntity<string>
public class Department : AuditableBaseEntity<int>
public class EmployeeStatus : AuditableBaseEntity<int>
```

### 2. Enums

#### Naming
```csharp
// ✅ Đúng
public enum TimeLogType
{
    CheckIn,
    CheckOut
}

public enum DisplayOrder
{
    Ascending,
    Descending
}

// ❌ Sai
public enum timeLogType
public enum displayOrder
public enum TimeLogTypes
```

## 🏗️ Infrastructure Layer Rules

### 1. DbContext

#### DbSet Naming
```csharp
// ✅ Đúng
public DbSet<DatEmployee> DatEmployees => Set<DatEmployee>();
public DbSet<MstDepartment> MstDepartments => Set<MstDepartment>();
public DbSet<KbnEmployeeStatus> KbnEmployeeStatus => Set<KbnEmployeeStatus>();

// ❌ Sai
public DbSet<DatEmployee> Employees => Set<DatEmployee>();
public DbSet<MstDepartment> Departments => Set<MstDepartment>();
```

#### Transaction Handling
```csharp
// ✅ Đúng
public async Task<IResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
{
    await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

    try
    {
        // Business logic implementation
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await Result.SuccessAsync(MessageConstants.AddSuccess);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(cancellationToken);
        return await Result.FailAsync($"Create failed: {ex.Message}");
    }
}
```

### 2. Services

#### Interface Implementation
```csharp
// ✅ Đúng
public class EmailService : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        // Implementation
    }
}

// ✅ Đúng - Sử dụng primary constructor
public class EmailService(IEmailConfiguration config) : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        // Implementation
    }
}
```

## 🌐 Web Layer Rules

### 1. Controllers

#### Base Controller Inheritance
```csharp
// ✅ Đúng
[ApiController]
[Route("api/user")]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    // Implementation
}

// ✅ Đúng - Kế thừa từ BaseController
[Authorize]
public class BaseController : Controller
{
    public string CurrentUserName => User.Identity?.Name ?? "Unknown";
    public string? CurrentUserId => User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    public List<string> CurrentUserPermissions => User.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
    public bool HasPermission(string permission) => User.HasClaim("permission", permission);
}
```

#### Action Methods
```csharp
// ✅ Đúng
[HttpGet("me")]
public async Task<IActionResult> GetCurrentUserInfo()
{
    var username = User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
        return new ObjectResult(GenericResponse<UserInfoDto>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
    
    var response = await mediator.Send(new GetUserProfileQuery { Username = username });
    return new ObjectResult(response) { StatusCode = response.Status };
}

[HttpPost("me")]
public async Task<IActionResult> PutCurrentUserInfo([FromBody] EmployeeGeneralDto request)
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors
            .Select(e => new ErrorDetail { Code = e.ErrorCode, Details = e.ErrorMessage })
            .ToList();

        return new ObjectResult(GenericResponse<object>.MultipleErrorsResponse(400, "", errors)) { StatusCode = 400 };
    }

    // Implementation
}
```

### 2. Middleware

#### Authorization Middleware
```csharp
// ✅ Đúng
public class BlacklistTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApplicationDbContext _context;

    public BlacklistTokenMiddleware(RequestDelegate next, IApplicationDbContext context)
    {
        _next = next;
        _context = context;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Implementation
        await _next(context);
    }
}
```

## 📝 Constants & Messages

### 1. Message Constants
```csharp
// ✅ Đúng
public static class MessageConstants
{
    public const string NotFound = "Không tìm thấy dữ liệu.";
    public const string UpdateSuccess = "Cập nhật thành công.";
    public const string UpdateFail = "Cập nhật thất bại.";
    public const string AddSuccess = "Tạo thành công.";
    public const string AddFail = "Tạo không thành công.";
    public const string DeleteSuccess = "Xóa thành công.";
    public const string DeleteFail = "Xóa không thành công.";
}
```

### 2. Error Code Constants
```csharp
// ✅ Đúng
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
        
        // Employee
        { "E001", "Invalid Employee." },
        { "E002", "Employee already exists." },
        { "E003", "Employee not found." },
    };

    public static string GetMessage(string code) => Errors.GetValueOrDefault(code, "Unknown error.");
}
```

## 🔄 AutoMapper Configuration

### 1. Mapping Configuration
```csharp
// ✅ Đúng
public class MappingConfiguration : Profile
{
    public MappingConfiguration()
    {
        #region System
        CreateMap<DateTime, string>().ConvertUsing<DateTimeToStringConverter>();
        CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeConverter>();
        #endregion

        #region Identity
        CreateMap<AppUser, UserDetailDto>().ReverseMap();
        CreateMap<AppRole, RoleDetailDto>().ReverseMap();
        CreateMap<AppRole, RoleResponse>().ReverseMap();
        #endregion

        #region Entity
        CreateMap<DatEmployee, EmployeeDetailDto>().ReverseMap();
        CreateMap<DatEmployee, EmployeeGeneralDto>().ReverseMap();
        CreateMap<DatCustomer, DatCustomerDto>().ReverseMap();
        #endregion

        #region Mst
        CreateMap<MstDepartment, DepartmentDetailDto>().ReverseMap();
        CreateMap<MstProgrammingLanguage, MstProgrammingLanguageDto>().ReverseMap();
        CreateMap<MstTeam, MstTeamDto>().ReverseMap();
        CreateMap<MstPosition, PositionDetailDto>().ReverseMap();
        #endregion

        #region Kbn
        CreateMap<KbnContractType, KbnContractTypeDto>().ReverseMap();
        CreateMap<KbnEmployeeStatus, KbnEmployeeStatusDto>().ReverseMap();
        CreateMap<KbnLeaveType, KbnLeaveTypeDto>().ReverseMap();
        #endregion
    }
}
```

## 🚀 Performance & Best Practices

### 1. Async/Await Pattern
```csharp
// ✅ Đúng
public async Task<IResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
{
    await using var transaction = await _context.BeginTransactionAsync(cancellationToken);
    
    try
    {
        var employee = _mapper.Map<DatEmployee>(request.Employee);
        employee.CreatedOn = DateTime.Now;
        _context.DatEmployees.Add(employee);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return await Result.SuccessAsync(MessageConstants.AddSuccess);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(cancellationToken);
        return await Result.FailAsync($"Create failed: {ex.Message}");
    }
}
```

### 2. Background Jobs
```csharp
// ✅ Đúng
BackgroundJob.Enqueue(() => _mailService.SendAsync(user.Email, subject, body));
```

### 3. Validation Pipeline
```csharp
// ✅ Đúng
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // Handle validation failures
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

## 📋 Code Review Checklist

### ✅ Bắt buộc kiểm tra
- [ ] Tuân thủ naming conventions
- [ ] Sử dụng async/await pattern đúng cách
- [ ] Có validation cho input data
- [ ] Sử dụng transaction khi cần thiết
- [ ] Có proper error handling
- [ ] Tuân thủ Clean Architecture principles
- [ ] Có unit tests cho business logic
- [ ] Sử dụng dependency injection đúng cách

### 🔍 Nên kiểm tra
- [ ] Code có dễ đọc và maintain không
- [ ] Có sử dụng constants thay vì hardcode strings
- [ ] Có proper logging
- [ ] Performance considerations
- [ ] Security best practices

## 🚫 Anti-patterns cần tránh

### ❌ Không được làm
- Hardcode connection strings, API keys
- Sử dụng synchronous methods trong async context
- Bỏ qua validation
- Sử dụng global variables
- Tight coupling giữa các layers
- Bỏ qua error handling
- Sử dụng magic numbers thay vì constants

## 📚 Tài liệu tham khảo

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [Entity Framework Core Best Practices](https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext)

---

**Lưu ý**: Tài liệu này cần được cập nhật thường xuyên để phù hợp với sự phát triển của dự án và các best practices mới nhất trong ngành.
