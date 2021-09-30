using Microsoft.EntityFrameworkCore;
using UrlService.Models;
using UrlService.Models.ModelsConfigurations;

namespace UrlService.Infrastructure
{
    public class UrlServiceDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<UrlInfo> UrlInfo { get; set; }
        public DbSet<RedirectRecord> RedirectRecord { get; set; }
        public DbSet<AnalyticsData> AnalyticsData { get; set; }

        public UrlServiceDbContext(DbContextOptions<UrlServiceDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration<User>(new UserEntityConfigurations());
            builder.ApplyConfiguration<UrlInfo>(new UrlInfoEntityConfigurations());
            builder.ApplyConfiguration<RedirectRecord>(new RedirectRecordEntityConfigurations());
            builder.ApplyConfiguration<AnalyticsData>(new AnalyticsDataEntityConfigurations());
            base.OnModelCreating(builder);
        }
    }
}