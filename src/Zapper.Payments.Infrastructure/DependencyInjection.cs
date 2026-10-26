using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Infrastructure.Acme;
using Zapper.Payments.Infrastructure.Persistence;
using Zapper.Payments.Infrastructure.Persistence.Repositories;

namespace Zapper.Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AcmeOptions>(
            configuration.GetSection("Acme"));

        var connectionString = configuration.GetConnectionString("PaymentsDB");

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PaymentsDB")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISubMerchantRepository, SubMerchantRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<PaymentsDbContext>());

        services.AddScoped<ICallbackSignatureVerifier,
            CallbackSignatureVerifier>();

        services.AddHttpClient<IAcmeClient, AcmeClient>(
                (sp, httpClient) =>
                {
                    var options = sp
                        .GetRequiredService<
                            Microsoft.Extensions.Options.IOptions<AcmeOptions>>();

                    httpClient.BaseAddress =
                        new Uri(options.Value.BaseUrl);
                })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 2;
                options.AttemptTimeout.Timeout =
                    TimeSpan.FromSeconds(5);

                options.TotalRequestTimeout.Timeout =
                    TimeSpan.FromSeconds(15);
            });

        return services;
    }
}