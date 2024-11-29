using MoneyTrackerWebApp;
using MoneyTrackerWebApp.Components;
using MoneyTrackerWebApp.Utils.SQLLogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

string logConnName = builder.Configuration.GetSection("Logging:Database").GetValue<string>("ConnName");
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddSQLLogger(configuration =>
{
    configuration.ConnectionString = builder.Configuration.GetConnectionString(logConnName);
    configuration.Default = builder.Configuration.GetSection("Logging:Database").GetValue<string>("Default").ToLogLevel();
});
/*
 * STILL NOT LOGGING TO CONSOLE
builder.Logging.AddMyConsoleLogger(configuration =>
{
#if DEBUG
    configuration.Default = LogLevel.Trace;
#else
    configuration.Default = builder.Configuration.GetSection("Logging:MyConsole").GetValue<string>("Default").ToLogLevel();
#endif
});
*/

SetupIOC.Configure(builder);



var app = builder.Build();

#if DEBUG
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogTrace(0, "Testing trace");
logger.LogDebug(1, "Testing debug");
logger.LogInformation(2, "Testing information");
logger.LogWarning(3, "Testing Warning");
logger.LogError(4, "Testing Error");

Console.WriteLine("WE ARE HERE!!!!!!!!!");
#endif


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
