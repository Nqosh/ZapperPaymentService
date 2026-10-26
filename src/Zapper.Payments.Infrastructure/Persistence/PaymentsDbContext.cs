using Microsoft.EntityFrameworkCore;
using Zapper.Payments.Application.Interfaces;
using Zapper.Payments.Domain.Entities;
using Zapper.Payments.Domain.Enums;

namespace Zapper.Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContext(
    DbContextOptions<PaymentsDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<StoredCard> StoredCards => Set<StoredCard>();

    public DbSet<SubMerchant> SubMerchants => Set<SubMerchant>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<PaymentEvent> PaymentEvents => Set<PaymentEvent>();

    public DbSet<AcmeNotification> AcmeNotifications => Set<AcmeNotification>();

    public DbSet<AcmeAttempt> AcmeAttempts => Set<AcmeAttempt>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>()
            .HasKey(x => x.Id);

        builder.Entity<Customer>()
            .HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Entity<StoredCard>()
            .HasKey(x => x.Id);

        builder.Entity<SubMerchant>()
            .HasKey(x => x.Id);

        builder.Entity<SubMerchant>()
            .HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Entity<Payment>()
            .HasKey(x => x.Id);

        builder.Entity<Payment>()
            .HasIndex(x => x.AppPaymentId)
            .IsUnique();

        builder.Entity<Payment>()
            .HasIndex(x => x.AcmePaymentId)
            .IsUnique();

        builder.Entity<Payment>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(x => x.Status)
            .HasConversion<string>();

        builder.Entity<PaymentEvent>()
            .HasKey(x => x.Id);

        builder.Entity<PaymentEvent>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Entity<PaymentEvent>()
            .Property(x => x.Type)
            .HasConversion<string>();

        builder.Entity<AcmeNotification>()
            .HasKey(x => x.Id);

        builder.Entity<AcmeNotification>()
            .HasIndex(x => x.NotificationId)
            .IsUnique();

        builder.Entity<AcmeAttempt>()
            .HasKey(x => x.Id);
    }
}