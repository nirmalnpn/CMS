using CMS.Api.Controllers.Comman;
using CMS.Modules.Modules;
using CMS.Modules.Modules.Settings;
using CMS.Shared;
using CMS.Shared.Services.Extensions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Bind the SmtpSettings from appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Register the SmtpClient as a scoped service
builder.Services.AddScoped(sp =>
{
    var smtpSettings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    var smtpClient = new SmtpClient();

    //// Use StartTls for port 587 with Gmail
    //smtpClient.Connect(smtpSettings.Host, smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

    //smtpClient.Authenticate(smtpSettings.UserName, smtpSettings.Password);
    return smtpClient;
});




//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
// Add services to the container.

// Bind the ResponseMsg section to the GlobalSettings
builder.Services.Configure<ResponseMsgModel>(builder.Configuration.GetSection("ResponseMsg"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<ResponseMsgModel>>().Value);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Log/log.text",rollingInterval:RollingInterval.Minute)
    .CreateLogger();
builder.Logging.ClearProviders();
//builder.Host.UseSerilog();  
builder.Logging.AddSerilog();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // This ensures 406 Not Acceptable is returned if the requested format is not available
})
.AddXmlDataContractSerializerFormatters(); // Adds XML formatter support
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomService();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Content-Disposition");
    });
});
var app = builder.Build();

// Access the configuration value
GlobalSettings.ResponseMessages = app.Services.GetRequiredService<ResponseMsgModel>();
// Register the custom exception middleware
app.UseMiddleware<CustomExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Use CORS before routing and endpoints
app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
