using AddressUtility.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AddressUtility.Context.Configurations
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable("RegionGroup");

            // Теневые свойства
            builder.Property<bool>("Deleted").HasDefaultValue(false);
            // TimeStamp не используется, но колонка в базе осталась. Когда-то давно разработчики договорились, что всё что добавлено после какого-то момента, в базу добавлять с TS = 10
            builder.Property<long>("FakeTimeStamp").HasColumnName("TS").HasDefaultValue(10);
            // В базе комментов к колонке нет. У Санкт-Петербурга и ЛО - 1, у Москвы и МО - 2, у всех остальных - 3
            // Нашел не связанную таблицу AddressGroups, в которой 1 - Север, 2 - Юг, 3 - Пригород.
            builder.Property<int>("MediaRegionGroupId").HasDefaultValue(3);
            builder.Property<int?>("RegionalCenterId");
            builder.HasKey(r => r.Id);
        }
    }
}
