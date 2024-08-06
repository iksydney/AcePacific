using AcePacific.Data.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AcePacific.API.ExtensionServices
{
    public static class MigrationsExtensions
    {

        public static void RunMigrations(this IServiceCollection services)
        {
            // Ensure the database is created and all pending migrations are applied
            using (var serviceScope = services.BuildServiceProvider().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    throw;

                }
            }
        }
    }
}
