using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using RaspberryPiWebServer.Data;
using RaspberryPiWebServer.Hubs;
using RaspberryPiWebServer.Models;
using RaspberryPiWebServer.Repository;
using RaspberryPiWebServer.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<Settings>(builder.Configuration.GetSection(nameof(Settings)));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

//builder.Services.AddSignalR();
//builder.Services.AddSingleton<MonitorService>();
//builder.Services.AddHostedService<MonitorWorker>();

builder.Services.AddScoped<IEntityRepository<Model>, ModelRepository>();
builder.Services.AddScoped<IEntityRepository<History>, HistoryRepository>();

builder.Services.AddScoped<IEntityService<Model>, ModelService>();
builder.Services.AddScoped<IEntityService<History>, HistoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseDeveloperExceptionPage();
app.UseMigrationsEndPoint();
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

//app.MapHub<MonitorHub>("/hubs/monitor");

app.Run();
