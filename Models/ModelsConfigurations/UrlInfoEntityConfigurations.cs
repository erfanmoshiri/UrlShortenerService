using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlService.Models.ModelsConfigurations
{
    public class UrlInfoEntityConfigurations : IEntityTypeConfiguration<UrlInfo>
    {
        public void Configure(EntityTypeBuilder<UrlInfo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getDate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue<bool>(false);

        }
    }


}