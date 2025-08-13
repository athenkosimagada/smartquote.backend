using smartquote.api.Settings;

namespace smartquote.api.Extensions;

public static class SettingsExtensions
{
    public static void ConfigureSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));
    }
}
