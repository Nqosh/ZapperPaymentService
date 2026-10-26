using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var apiKey =
    builder.Configuration["Acme:ApiKey"]
    ?? "local-api-key";

var secret =
    builder.Configuration["Acme:CallbackSecret"]
    ?? "local-callback-secret";

var callbackBase =
    builder.Configuration["Acme:CallbackBaseUrl"]
    ?? "http://localhost:8080";

var ids = new ConcurrentDictionary<string, string>();

app.MapPost(
    "/payments",
    async (
        HttpRequest request,
        StubPayment p,
        IHttpClientFactory clients,
        CancellationToken ct) =>
    {

        Console.WriteLine($"Received: {request.Headers["X-Api-Key"]}");
        Console.WriteLine($"Expected: {apiKey}");


        if (request.Headers["X-Api-Key"] != apiKey)
        {
            Console.WriteLine("AUTH FAILED");
            Console.WriteLine($"Received: '{request.Headers["X-Api-Key"]}'");
            Console.WriteLine($"Expected: '{apiKey}'");

            return Results.Unauthorized();
        }

        if (request.Headers["X-Api-Key"] != apiKey)
        {
            return Results.Unauthorized();
        }

        var id = ids.GetOrAdd(
            p.IdempotencyKey,
            _ => $"acme-{Guid.NewGuid():N}");

        var mode = p.IdempotencyKey.ToLowerInvariant();

        _ = Task.Run(async () =>
        {
            await Task.Delay(
                mode.Contains("timeout")
                    ? 4000
                    : 300);

            if (mode.Contains("decline"))
            {
                await Send("Declined");
            }
            else if (mode.Contains("reverse"))
            {
                await Send("Approved");

                await Task.Delay(300);

                await Send("Reversed");
            }
            else if (mode.Contains("refund"))
            {
                await Send("Approved");

                await Task.Delay(300);

                await Send("Refunded");
            }
            else if (mode.Contains("outoforder"))
            {
                await Send("Refunded");

                await Task.Delay(300);

                await Send("Approved");
            }
            else if (mode.Contains("duplicate"))
            {
                var notificationId =
                    Guid.NewGuid().ToString("N");

                await Send("Approved", notificationId);
                await Send("Approved", notificationId);
            }
            else
            {
                await Send("Approved");
            }

            async Task Send(
                string eventType,
                string? notificationId = null)
            {
                var body = JsonSerializer.Serialize(
                    new
                    {
                        notificationId =
                            notificationId
                            ?? Guid.NewGuid().ToString("N"),

                        acmePaymentId = id,
                        appPaymentId = p.IdempotencyKey,
                        eventType,
                        occurredAt = DateTimeOffset.UtcNow
                    },
                    new JsonSerializerOptions(
                        JsonSerializerDefaults.Web));

                var signature =
                    Convert.ToHexString(
                            HMACSHA256.HashData(
                                Encoding.UTF8.GetBytes(secret),
                                Encoding.UTF8.GetBytes(body)))
                        .ToLowerInvariant();

                using var message = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{callbackBase}/api/acme/notifications")
                {
                    Content = new StringContent(
                        body,
                        Encoding.UTF8,
                        "application/json")
                };

                message.Headers.Add(
                    "X-Acme-Signature",
                    signature);

                try
                {
                    await clients
                        .CreateClient()
                        .SendAsync(message);
                }
                catch
                {
                    // Intentionally ignored in stub
                }
            }
        });

        if (mode.Contains("timeout"))
        {
            await Task.Delay(10000, ct);
        }

        return Results.Accepted(
            value: new
            {
                paymentId = id,
                status = "Pending"
            });
    });

app.MapGet(
    "/health",
    () => Results.Ok(
        new
        {
            status = "ok"
        }));

await app.RunAsync();

public sealed record StubPayment(
    string IdempotencyKey,
    decimal Amount,
    string Currency,
    string CardToken,
    string CustomerEmail,
    object Merchant);