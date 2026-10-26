using Microsoft.Extensions.DependencyInjection;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Application.Services;

namespace Zapper.Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        return services
            .AddScoped<IPaymentService, PaymentService>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IReportingService, ReportingService>();
    }
}