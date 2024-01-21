using TimeServer;
using TimeServer.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure();

builder.SetupKestrel();

// Run as Windows service
//builder.Host.UseWindowsService();
//builder.Services.AddWindowsService();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<TimeServer.Services.TimeService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();
