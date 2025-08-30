using smartquote.api.Options;

namespace smartquote.api.Extensions;

public static class OptionsExtensions
{
    public static void ConfigureSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("JwtOptions"));

        builder.Services.Configure<EmailOptions>(
            builder.Configuration.GetSection("EmailOptions"));
    }
}
