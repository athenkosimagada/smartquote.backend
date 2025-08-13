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
}
