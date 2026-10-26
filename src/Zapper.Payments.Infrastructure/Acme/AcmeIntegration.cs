using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Zapper.Payments.Application.Contracts;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;

namespace Zapper.Payments.Infrastructure.Acme;

public sealed class AcmeOptions
{
    public string BaseUrl { get; set; } = "";

    public string ApiKey { get; set; } = "";

    public string CallbackSecret { get; set; } = "";

    public int RequestTimeoutSeconds { get; set; } = 3;

    public int MaxConcurrency { get; set; } = 20;
}

public sealed record AcmeMerchant(
    string Id,
    string TradingName,
    string City,
    string Region,
    string Country,
    string Mcc,
    string Acquirer);

public sealed record AcmePaymentRequest(
    string IdempotencyKey,
    decimal Amount,
    string Currency,
    string CardToken,
    string CustomerEmail,
    AcmeMerchant Merchant);

public sealed record AcmeResponse(
    string PaymentId,
    string Status);

public sealed class AcmeClient(
    HttpClient http,
    IOptions<AcmeOptions> options)
    : IAcmeClient
{
    private readonly AcmeOptions _options = options.Value;

    private readonly SemaphoreSlim _gate =
        new(Math.Max(1, options.Value.MaxConcurrency));

    public async Task<AcmeSubmissionResult> SubmitAsync(
        Payment payment,
        StoredCard card,
        SubMerchant merchant,
        CancellationToken ct)
    {
        await _gate.WaitAsync(ct);

        try
        {
            using var timeout =
                CancellationTokenSource.CreateLinkedTokenSource(ct);

            timeout.CancelAfter(
                TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            var body = new AcmePaymentRequest(
                payment.AppPaymentId,
                payment.Amount,
                payment.Currency,
                card.AcmeToken,
                payment.Customer.Email,
                new AcmeMerchant(
                    merchant.ExternalId,
                    merchant.TradingName,
                    merchant.City,
                    merchant.Region,
                    merchant.Country,
                    merchant.Mcc,
                    "FNB"));

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/payments")
            {
                Content = JsonContent.Create(body)
            };

            request.Headers.Add(
                "X-Api-Key",
                _options.ApiKey);

            request.Headers.Add(
                "Idempotency-Key",
                payment.AppPaymentId);

            try
            {
                using var response =
                    await http.SendAsync(
                        request,
                        timeout.Token);

                var raw =
                        await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new AcmeSubmissionResult(
                        false,
                        false,
                        null,
                        (int)response.StatusCode,
                        null);
                }

                var data = JsonSerializer.Deserialize<AcmeResponse>(
                            raw,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                return response.IsSuccessStatusCode
                    ? new AcmeSubmissionResult(
                        true,
                        false,
                        data?.PaymentId,
                        (int)response.StatusCode,
                        null)
                    : new AcmeSubmissionResult(
                        false,
                        false,
                        null,
                        (int)response.StatusCode,
                        $"Acme returned {(int)response.StatusCode}");
            }
            catch (OperationCanceledException)
                when (!ct.IsCancellationRequested)
            {
                return new AcmeSubmissionResult(
                    false,
                    true,
                    null,
                    null,
                    "Provider timeout");
            }
            catch (HttpRequestException ex)
            {
                return new AcmeSubmissionResult(
                    false,
                    false,
                    null,
                    null,
                    ex.Message);
            }
        }
        finally
        {
            _gate.Release();
        }
    }
}

public sealed class CallbackSignatureVerifier(
    IOptions<AcmeOptions> options)
    : ICallbackSignatureVerifier
{
    public bool IsValid(
        string raw,
        string? signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        Console.WriteLine($"Received Test Signature: {signature}");

        var expected = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(options.Value.CallbackSecret),
            Encoding.UTF8.GetBytes(raw));

        Console.WriteLine($"Expected Test Signature: {expected}");
        try
        {
            return CryptographicOperations.FixedTimeEquals(
                expected,
                Convert.FromHexString(signature));
        }
        catch (FormatException)
        {
            return false;
        }
    }
}