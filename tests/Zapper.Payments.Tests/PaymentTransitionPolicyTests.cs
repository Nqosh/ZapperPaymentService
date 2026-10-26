using Xunit;
using Zapper.Payments.Domain.Entities;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Tests;

public sealed class PaymentTransitionPolicyTests
{
    [Theory]
    [InlineData(PaymentStatus.Pending, PaymentStatus.Approved, true)]
    [InlineData(PaymentStatus.Pending, PaymentStatus.Declined, true)]
    [InlineData(PaymentStatus.Pending, PaymentStatus.Refunded, true)]
    [InlineData(PaymentStatus.Approved, PaymentStatus.Refunded, true)]
    [InlineData(PaymentStatus.Refunded, PaymentStatus.Approved, false)]
    [InlineData(PaymentStatus.Declined, PaymentStatus.Approved, false)]
    public void Enforces_monotonic_transitions(
        PaymentStatus current,
        PaymentStatus incoming,
        bool expected)
        => Assert.Equal(
            expected,
            PaymentTransitionPolicy.CanTransition(current, incoming));
}