using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using smartquote.api.Entities;

namespace smartquote.api.Data;

public class SmartQuoteDbContext : IdentityDbContext<User>
{
    public SmartQuoteDbContext(DbContextOptions<SmartQuoteDbContext> options)
        : base(options) { }

    public DbSet<Item> Items { get; set; }
    public DbSet<Quote> Quotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add unique index on Email
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_User_Email");
    }
}
