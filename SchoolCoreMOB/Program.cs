using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Services;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
#region Localization
//Step 1
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("SharedResource", assemblyName.Name);
        };
    });

builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new List<CultureInfo>
            {
                            new CultureInfo("ar-SA"),
                            new CultureInfo("en-US")
            };

        options.DefaultRequestCulture = new RequestCulture(culture: "ar-SA", uiCulture: "ar-SA");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    });
#endregion
var connectionstring = builder.Configuration.GetConnectionString("APIConnection");

//builder.Services.AddDbContext<APIContext>(options =>
//{
//    options.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring));
//});
builder.Services.AddDistributedMemoryCache(); // Use distributed memory cache for session storage
                                              // Add a persistent data protection key store (shared key store)
                                              //builder.Services.AddDataProtection()
                                              //        .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Keys"))  // Change to a suitable path
                                              //        .SetApplicationName("SchoolCoreMOB")  // Optional: Name for the key ring
                                              //        .ProtectKeysWithDpapi();  // Optional: Encrypt keys using DPAPI (Windows)

// Configure Kestrel to listen on specific IP and port (for development or testing)
//builder.WebHost.ConfigureKestrel(options =>
//{
//    // Listen on all IPs (0.0.0.0) on port 5001 for HTTPS or 5000 for HTTP
//    options.Listen(IPAddress.Any, 5001, listenOptions =>
//    {
//        listenOptions.UseHttps("C:\\Users\\intel\\Desktop\\pwaMOB.pfx", "123"); // Use the certificate
//    });
//    options.Listen(IPAddress.Any, 5000); // HTTP for non-SSL
//});

// Add services to the container.
//builder.Services.AddFido2(options =>
//{
//    options.ServerDomain = "localhost";
//    options.ServerName = "SchoolCoreMOB";
//    // options.TimestampDriftTolerance = TimeSpan.FromMinutes(5);
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


});
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None
    });
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromDays(365);//You can set Time   
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add<SessionCheckFilter>();
//});
builder.Services.AddScoped<ICallService, CallService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
// Register custom session check middleware
//app.UseMiddleware<SessionCheckFilter>();

app.UseHttpsRedirection();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
