using Microsoft.EntityFrameworkCore;
using smartquote.api.Data;

namespace smartquote.api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using SmartQuoteDbContext context =
            scope.ServiceProvider.GetRequiredService<SmartQuoteDbContext>();

        context.Database.Migrate();
    }
}
