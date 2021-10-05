using AddressUtility.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace AddressUtility.Context.Configurations
{
    public class AddressObjectConfiguration : IEntityTypeConfiguration<AddressObject>
    {
        public void Configure(EntityTypeBuilder<AddressObject> builder)
        {
            builder.ToTable("AddressObj");
            builder.Property(o => o.Id).HasColumnName("ObjectId");
            builder.Property(o => o.ParentObjectId).HasColumnName("ParentObj");
            builder.Property(o => o.RegionId).HasColumnName("RegionGroupId");
            builder.Property(o => o.TypeId).HasColumnName("ObjectType");
            builder.Property(o => o.AtomId).HasColumnName("ObjectAtom");

            // Теневые свойства
            builder.Property<bool>("Deleted").HasDefaultValue(false);
            // TimeStamp не используется, но колонка в базе осталась. Когда-то давно разработчики договорились, что всё что добавлено
            // после какого-то момента, в базу добавлять с TS = 10
            builder.Property<long>("FakeTimeStamp").HasColumnName("TS").HasDefaultValue(10);
            builder.Property<string>("ShortName").HasMaxLength(100);
            builder.Property<int?>("SubstObj");
            builder.Property<Point>("Coordinates");

            builder.HasKey(o => o.Id);
            builder.HasOne(o => o.AddrType)
                   .WithMany(t => t.AddressObjects)
                   .HasForeignKey(o => o.TypeId);
            builder.HasOne(o => o.Atom)
                   .WithMany(a => a.AddressObjects)
                   .HasForeignKey(o => o.AtomId);
            builder.HasOne(o => o.Region)
                   .WithMany(r => r.AddressObjects)
                   .HasForeignKey(o => o.RegionId);
        }
    }
}
