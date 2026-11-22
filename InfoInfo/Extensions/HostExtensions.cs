using InfoInfo.Data;

namespace InfoInfo.Extensions
{
    public static class HostExtensions
    {
        public static async Task SeedData(this IHost host)
        {
            try
            {
                await InfoSeeder.Initialize(host.Services);
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "Wystąpił błąd podczas wypełniania bazy danych.");
            }
        }
    }
}
