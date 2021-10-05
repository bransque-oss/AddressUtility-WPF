using AddressUtility.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AddressUtility.Context.Configurations
{
    public class AddrTypeConfiguration : IEntityTypeConfiguration<AddrType>
    {
        public void Configure(EntityTypeBuilder<AddrType> builder)
        {
            builder.ToTable("AddressObjTypes");
            builder.Property(t => t.Id).HasColumnName("ObjTypeId");
            builder.Property(t => t.Name).HasColumnName("TypeName");

            // Теневые свойства
            builder.Property<bool>("Deleted").HasDefaultValue(false);
            // TimeStamp не используется, но колонка в базе осталась. Когда-то давно разработчики договорились, что всё что добавлено
            // после какого-то момента, в базу добавлять с TS = 10
            builder.Property<long>("TimeStamp").HasColumnName("TS").HasDefaultValue(10);

            builder.HasKey(t => t.Id);
        }
    }
}
