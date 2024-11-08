using MoneyTrackerWeb;
using MoneyTrackerWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// Define the config file to use
builder.Configuration
    .AddJsonFile("appsettings.json");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

SetupIOC.Configure(builder);


var app = builder.Build();

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
