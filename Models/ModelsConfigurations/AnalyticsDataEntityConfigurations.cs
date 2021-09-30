using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlService.Models.ModelsConfigurations
{
    public class AnalyticsDataEntityConfigurations : IEntityTypeConfiguration<AnalyticsData>
    {
        public void Configure(EntityTypeBuilder<AnalyticsData> builder)
        {
            // builder.ToTable("testTable");
            builder.HasKey(x => x.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getDate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue<bool>(false);

        }
    }


}