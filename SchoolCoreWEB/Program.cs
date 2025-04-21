
using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Services;
using System.Globalization;
using System.Reflection;



//using jsreport.AspNetCore;

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
builder.Services.AddDistributedMemoryCache(); // Use distributed memory cache for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365);//You can set Time   
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
// Add services to the container.
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
builder.Services.AddControllersWithViews();
//builder.Services.AddJsReport(options =>
//{
//    options.WebEngine.Enabled = true;
//});
// Configure jsreport
//builder.Services.AddSingleton<IJsReportLocal, JsReportLocal>(serviceProvider =>
//{
//    var jsreport = new JsReportLocal()
//    .Configure(cfg =>
//    {
//        cfg.SetBinary(j => j.UseBinary());
//        cfg.SetTemplatesStoreInMemory();
//    });
//    jsreport.Init();
//    return jsreport;
//});

//builder.Services.AddJsReport()
//    .Configure(cfg =>
//    {
//        cfg.AsLocal()  // Use jsReport locally (no server required)
//            .Configure(localCfg =>
//            {
//                localCfg.TemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");  // Template folder

//                // Set default engine and recipe
//                localCfg.Engine = Engine.Handlebars;    // Template engine
//                localCfg.Recipe = Recipe.ChromePdf;     // Recipe for generating PDFs
//            });
//    })
//    .Create();  // Finalize the setup
builder.Services.AddScoped<ICallService, CallService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseResponseCaching();

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            NoStore = true,
            NoCache = true,
            MaxAge = TimeSpan.FromSeconds(0)
        };

    await next();
});



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseSession();
//app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
