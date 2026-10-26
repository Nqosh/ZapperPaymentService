using System.Text;
using Microsoft.AspNetCore.Mvc;
using Zapper.Payments.Application;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Application.Services;
using Zapper.Payments.Infrastructure;
using Zapper.Payments.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<PaymentsDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks(
    "/health/live",
    new()
    {
        Predicate = _ => false
    });

app.MapHealthChecks("/health/ready");

app.MapPost(
        "/api/payments",
        async (
            [FromBody] CreatePaymentRequest request,
            IPaymentService service,
            CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.Error });
            }

            if (result.Created)
            {
                return Results.Created(
                    $"/api/payments/{result.Value!.AppPaymentId}",
                    result.Value);
            }

            return Results.Ok(result.Value);
        })
    .WithOpenApi();

app.MapGet(
        "/api/payments/{appPaymentId}",
        async (
            string appPaymentId,
            IPaymentService service,
            CancellationToken ct) =>
        {
            var result = await service.GetAsync(appPaymentId, ct);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
    .WithOpenApi();

app.MapPost(
        "/api/acme/notifications",
        async (
            HttpRequest request,
            INotificationService service,
            CancellationToken ct) =>
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);

            var raw = await reader.ReadToEndAsync(ct);

            if (!service.IsValidSignature(
                    raw,
                    request.Headers["X-Acme-Signature"].FirstOrDefault()))
            {
                return Results.Unauthorized();
            }

            return Results.Ok(
                new
                {
                    result = await service.ProcessAsync(raw, ct)
                });
        })
    .ExcludeFromDescription();

app.MapGet(
        "/api/reports/daily",
        async (
            [FromQuery] DateOnly date,
            IReportingService service,
            CancellationToken ct) =>
                Results.Ok(await service.GetDailyAsync(date, ct)))
    .WithOpenApi();

await using (var scope = app.Services.CreateAsyncScope())
{
    await SeedData.InitialiseAsync(
        scope.ServiceProvider.GetRequiredService<PaymentsDbContext>());
}

await app.RunAsync();

public partial class Program
{
}