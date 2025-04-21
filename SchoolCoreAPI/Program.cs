using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using SchoolCoreAPI.Configurations;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using SchoolCoreAPI.Services;
using System.Reflection;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add logging services
builder.Services.AddLogging();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());


builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//builder.Services.AddHangfire(config =>
//    config.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new MySqlStorageOptions())));
builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3"));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Connection"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});
builder.Services.AddDbContext<APIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection"))); // Use your PostgreSQL connection string here


builder.Services.AddHangfireServer();


//builder.Services.AddRateLimiter(_ => _
//    .AddFixedWindowLimiter(policyName: "fixed", options =>
//    {
//        options.PermitLimit = 1;
//        options.Window = TimeSpan.FromSeconds(5);
//        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        options.QueueLimit = 0;
//    }).RejectionStatusCode = StatusCodes.Status429TooManyRequests
//    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});



builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddSignalR();


builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddAuthorization();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDataProtection(); // For encrypting/decrypting keys
builder.Services.AddSingleton<HostEnvironmentService>();
builder.Services.AddSingleton<S3Service>();

builder.Services.AddScoped<IVapidKeyService, VapidKeyService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPushSubscriptionService, PushSubscriptionService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IAttendenceService, AttendenceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportSettingsService, ReportSettingsService>();
builder.Services.AddScoped<IScheduleSyllabusService, ScheduleSyllabusService>();
builder.Services.AddScoped<IScheduleResultService, ScheduleResultService>();
builder.Services.AddScoped<IExamTimeService, ExamTimeService>();
builder.Services.AddScoped<IExamTypeService, ExamTypeService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IQuestionCategoryService, QuestionCategoryService>();
builder.Services.AddScoped<IQuestionDifficultyService, QuestionDifficultyService>();
builder.Services.AddScoped<IQuestionTypeService, QuestionTypeService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();
builder.Services.AddScoped<IReportCardService, ReportCardService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IMediumService, MediumService>();
builder.Services.AddScoped<IBranchClassService, BranchClassService>();
builder.Services.AddScoped<ISessionYearService, SessionYearService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IStudentTransferService, StudentTransferService>();
builder.Services.AddScoped<IBloodGroupService, BloodGroupService>();
builder.Services.AddScoped<ITeacherTransferService, TeacherTransferService>();
builder.Services.AddScoped<IFeeTypeService, FeeTypeService>();
builder.Services.AddScoped<IEnquiryStatusService, EnquiryStatusService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddScoped<IReferenceAdmissionService, ReferenceAdmissionService>();
builder.Services.AddScoped<IStudentEnquiryService, StudentEnquiryService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStudentAssignmentService, StudentAssignmentService>();
builder.Services.AddScoped<ITeacherAnnouncementService, TeacherAnnouncementService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFeeStructureService, FeeStructureService>();
builder.Services.AddScoped<ICalendarEventService, CalendarEventService>();
builder.Services.AddScoped<IAdminAnnouncementService, AdminAnnouncementService>();
builder.Services.AddScoped<IPayModeService, PayModeService>();
builder.Services.AddScoped<IAdminNotificationService, AdminNotificationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IPeriodBreakService, PeriodBreakService>();
builder.Services.AddScoped<IMbPersonnelService, MbPersonnelService>();
builder.Services.AddScoped<IStudentDueService, StudentDueService>();
builder.Services.AddScoped<IProgressReportService, ProgressReportService>();
builder.Services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("../swagger/v1/swagger.json", "v1");
    });
}
// Register the logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();
//app.UseMiddleware<RateLimitMiddleware>();
//app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire"); // Expose dashboard at /hangfire

//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    Authorization = new[] { new MyHangfireAuthorizationFilter() }
//});

app.MapControllers();

// Schedule the database backup job
RecurringJob.AddOrUpdate<IDatabaseBackupService>(
    "daily-db-backup",
    service => service.PerformBackup(),
    Cron.Daily(1, 45));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WeatherForecast}/{action=GetWeatherForecast}/{id?}");

//static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

//app.MapGet("/", () => Results.Ok($"Hello {GetTicks()}"))
//                           .RequireRateLimiting("fixed");

app.Run();


