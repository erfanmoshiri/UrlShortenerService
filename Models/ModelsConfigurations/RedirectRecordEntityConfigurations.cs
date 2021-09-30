using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlService.Models.ModelsConfigurations
{
    public class RedirectRecordEntityConfigurations : IEntityTypeConfiguration<RedirectRecord>
    {
        public void Configure(EntityTypeBuilder<RedirectRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getDate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue<bool>(false);

        }
    }


}