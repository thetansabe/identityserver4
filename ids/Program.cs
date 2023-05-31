using ids;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
               .MinimumLevel.Override("System", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
               .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddIdentityServer()
//                .AddInMemoryClients(Config.Clients)
//                .AddInMemoryIdentityResources(Config.IdentityResources)
//                .AddInMemoryApiResources(Config.ApiResources)
//                .AddInMemoryApiScopes(Config.ApiScopes)
//                .AddTestUsers(Config.Users)
//                .AddDeveloperSigningCredential();

var sqlConnStr = builder.Configuration.GetConnectionString("ERP_db");

builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlServer(
        sqlConnStr,
        d => {
            d.MigrationsAssembly("ids");
            d.MigrationsHistoryTable("__EFMigrationsHistory", "Identity"); 
        }
    );
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        sqlConnStr,
                        d => {
                            d.MigrationsAssembly("ids");
                            d.MigrationsHistoryTable("__EFMigrationsHistory", "IDS4"); 
                        });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        sqlConnStr,
                        d => {
                            d.MigrationsAssembly("ids");
                            d.MigrationsHistoryTable("__EFMigrationsHistory", "IDS4");
                        });
                })
                .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//seeding
var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
};
if (seed)
{
    Log.Information("Seeding database...");
    var config = app.Services.GetRequiredService<IConfiguration>();
    SeedData.EnsureSeedData(sqlConnStr);
    Log.Information("Done seeding database.");
    return;
}

Log.Information("Starting host...");


app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
