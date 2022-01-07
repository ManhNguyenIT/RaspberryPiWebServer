using Microsoft.AspNetCore.HttpOverrides;
using RaspberryPiWebServer.Hubs;
using RaspberryPiWebServer.Models;
using RaspberryPiWebServer.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<Settings>(builder.Configuration.GetSection(nameof(Settings)));

builder.Services.AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddSignalR();
builder.Services.AddSingleton<MonitorService>();
builder.Services.AddHostedService<MonitorWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.MapHub<MonitorHub>("/hubs/monitor");

app.Run();
