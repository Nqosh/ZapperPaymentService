using Microsoft.EntityFrameworkCore;
using Zapper.Payments.Domain.Entities;
using Zapper.Payments.Infrastructure.Persistence;

namespace Zapper.Payments.Infrastructure;

public static class SeedData
{
    public static async Task InitialiseAsync(
        PaymentsDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Customers.AnyAsync())
        {
            return;
        }

        var customer1 = new Customer(
            "customer-001",
            "customer1@example.test");

        var customer2 = new Customer(
            "customer-002",
            "customer2@example.test");

        customer1.StoredCards.Add(
            new StoredCard(
                customer1,
                "tok_acme_customer_001"));

        customer2.StoredCards.Add(
            new StoredCard(
                customer2,
                "tok_acme_customer_002"));

        db.Customers.AddRange(
            customer1,
            customer2);

        db.SubMerchants.AddRange(
            new SubMerchant(
                "merchant-001",
                "Jozi Coffee",
                "Johannesburg",
                "Gauteng",
                "ZA",
                "5814"),

            new SubMerchant(
                "merchant-002",
                "Cape Books",
                "Cape Town",
                "Western Cape",
                "ZA",
                "5942"));

        await db.SaveChangesAsync();
    }
}