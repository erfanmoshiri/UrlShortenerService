using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UrlService.Infrastructure
{
    public static class MigrationsConfiguration
    {
        public static void ApplyMigrations(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<UrlServiceDbContext>().Database.Migrate();
            }
        }
    }
}