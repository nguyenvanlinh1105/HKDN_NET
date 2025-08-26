using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NineERP.Application.Configurations;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Identity;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Mappings;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Authorization;
using NineERP.Infrastructure.Contexts;
using NineERP.Infrastructure.Services.Common;
using NineERP.Infrastructure.Services.Identity;
using NineERP.Infrastructure.Services.SecurityStamp;
using reCAPTCHA.AspNetCore;
using Serilog;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Amazon;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NineERP.Application.Wrapper;
using NineERP.Web.Authorization;
using CurrentUserService = NineERP.Infrastructure.Services.Identity.CurrentUserService;
using Amazon.SimpleNotificationService;
using NineERP.Application.Interfaces.AwsNotification;
using NineERP.Infrastructure.Services.AwsNotification;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using FluentValidation;
using NineERP.Application.Dtos.BankSalary;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Validator;
using NineERP.Application.Dtos.EmergencyContact;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.LeaveCalculation;
using NineERP.Infrastructure.Services.LeaveCalculation;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// 🔠 Localization
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedCultures = new[] { new CultureInfo("vi-VN"), new CultureInfo("en-US") };
    opts.DefaultRequestCulture = new RequestCulture("vi-VN");
    opts.SupportedCultures = supportedCultures;
    opts.SupportedUICultures = supportedCultures;
    opts.RequestCultureProviders = [new CookieRequestCultureProvider()];
});

// 👁️ Razor + Localization
services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization()
    .AddRazorRuntimeCompilation();
services.AddRazorPages();

// 🍪 Cookie Policy
services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// 🔐 Identity + EF Core
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("Session:DefaultLockoutTimeSpan");
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Session:DefaultLockoutTimeSpan"));
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 🧁 Cookie
services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 🧾 Jwt auth
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

// 🔐 Authentication
services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            // 🔥 BLOCK TOKEN DISABLED
            OnTokenValidated = async context =>
            {
                var path = context.HttpContext.Request.Path.Value;

                if (!string.IsNullOrEmpty(path) && path.Contains("/api/auth/logout"))
                {
                    return;
                }
                await BlacklistTokenValidator.ValidateToken(context);
            },
            OnAuthenticationFailed = context =>
            {
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized) return Task.CompletedTask;
                context.NoResult();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = GenericResponse<object>.ErrorResponse(
                    401,
                    "Unauthorized access",
                    "AUTH005",
                    "Unauthorized access"
                );

                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },

            OnForbidden = context =>
            {
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden) return Task.CompletedTask;
                context.NoResult();
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = GenericResponse<object>.ErrorResponse(
                    403,
                    "Forbidden",
                    "AUTH008",
                    "You do not have permission to access this resource."
                );

                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },

            OnChallenge = context =>
            {
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized || context.Response.StatusCode == StatusCodes.Status403Forbidden) return Task.CompletedTask;
                context.HandleResponse();
                context.Response.ContentType = "application/json";

                var response = GenericResponse<object>.ErrorResponse(
                    401,
                    "Unauthorized access",
                    "AUTH005",
                    "Unauthorized access"
                );
                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });

// 🛡️ Authorization
services.AddAuthorization();
services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// 🛠 Config Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ERP API",
        Version = "v1"
    });

    // 🔐 Thêm Authentication vào Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token: your_token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.DocInclusionPredicate((_, apiDesc) => apiDesc.ActionDescriptor is ControllerActionDescriptor controllerAction &&
                                                        controllerAction.ControllerTypeInfo.Namespace != null &&
                                                        controllerAction.ControllerTypeInfo.Namespace.Contains("NineERP.Web.Controllers.Api"));
});

// 🔁 Session
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(configuration.GetValue<int>("Session:IdleTimeoutHours"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ⚙️ App Services
services.AddScoped<ICurrentUserService, CurrentUserService>();
services.AddScoped<CurrentUserService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
services.AddScoped<IDateTimeService, DateTimeService>();
services.AddScoped<ITokenService, IdentityService>();
services.AddScoped<ILeaveCalculationService, LeaveCalculationService>();
services.AddScoped<IValidator<EmployeeGeneralDto>, EmployeeGeneralDtoValidator>();
services.AddScoped<IValidator<EmergencyContactDto>, EmergencyContactDtoValidator>();
services.AddScoped<IValidator<BankDto>, BankDtoValidator>();
services.AddScoped<IValidator<AddCommentTaskDto>, AddCommentTaskDtoValidator>();
var awsOptions = new AWSOptions
{
    Credentials = new BasicAWSCredentials(
        builder.Configuration["AWS:AccessKey"],
        builder.Configuration["AWS:SecretKey"]
    ),
    Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"])
};
services.AddDefaultAWSOptions(awsOptions);
services.AddAWSService<IAmazonSimpleNotificationService>();
services.AddSingleton<INotificationService, AwsNotificationService>();

services.AddScoped<IAuditLogService, AuditLogService>();
services.AddScoped<TokenBlacklistService>();
services.AddScoped<TokenCleanupService>();
services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
services.AddScoped<SecurityStampCacheService>();

// ☁️ Integration Uploader
services.AddScoped<IGoogleDriveUploader, GoogleDriveUploader>();
services.AddScoped<IS3Uploader, S3Uploader>();
services.AddScoped<IIntegrationUploader, IntegrationUploader>();

// 🔄 Utilities
services.AddMemoryCache();
services.AddAutoMapper(typeof(MappingConfiguration).Assembly);
services.AddHttpContextAccessor();
services.AddHttpClient(); // 🧠 Cần cho ValidateReCaptchaKeyCommand và các lệnh test khác

// 📧 Email Config
services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));

// 🧪 Recaptcha
services.AddRecaptcha(options => configuration.GetSection("Recaptcha").Bind(options));

// 📦 Media tR
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining(typeof(NineERP.Application.AssemblyReference)));

// ⏱️ Hangfire
services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.FromSeconds(15),
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true
          });
});
services.AddHangfireServer();

// 📄 Logging
builder.Host.UseSerilog((ctx, logConfig) =>
{
    logConfig.ReadFrom.Configuration(ctx.Configuration);
});

var app = builder.Build();

// 🧪 Seed DB
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    seeder.Initialize();
}

// 🔧 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Exception");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
else
{
    // 🔥 Activate Swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NineERP API v1");
    });
}

app.UseStaticFiles();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseRouting();
app.UseCookiePolicy();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
