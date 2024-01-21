using TimeServer.Application.Exceptions;
using TimeServer.Application.Interfaces;
using TimeServer.DataAccess;
using TimeServer.Infrastructure;

namespace TimeServer;

internal static class DependencyInjection
{
    internal static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString(Constants.DbConnectionString)
                ?? throw new InvalidConfigurationException();

            options.UseSqlite(connectionString);
        });

        services
            .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options =>
            {
                options.AllowedCertificateTypes = CertificateTypes.All;
                options.Events = new()
                {
                    OnCertificateValidated = context =>
                    {
                        var claims = new[]
                        {
                            new Claim(
                                ClaimTypes.Name,
                                context.ClientCertificate.Subject,
                                ClaimValueTypes.String,
                                context.Options.ClaimsIssuer)
                        };

                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = async context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(context.Exception.ToString());
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }

    internal static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>();
        services.AddScoped<ITimeProvider, AuditableTimeProvider>();

        return services;
    }

    internal static void SetupKestrel(this WebApplicationBuilder builder)
    {
        string certPath = builder.Configuration["CertPath"]
            ?? throw new KeyNotFoundException("CertPath");

        string certPassword = builder.Configuration["CertPassword"]
            ?? throw new KeyNotFoundException("CertPassword");

        builder.WebHost.ConfigureKestrel(options =>
        {
            X509Certificate2 certificate = new(certPath, certPassword);
            options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            options.ListenAnyIP(5001, o =>
            {
                o.UseHttps(certificate);
                o.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.ConfigureHttpsDefaults(o =>
            {
                o.ServerCertificate = certificate;
                o.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
            });
        });
    }
}
