using AddressUtility.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AddressUtility.Context.Configurations
{
    public class AtomConfiguration : IEntityTypeConfiguration<Atom>
    {
        public void Configure(EntityTypeBuilder<Atom> builder)
        {
            builder.ToTable("AddressObjAtoms");
            builder.Property(a => a.Id).HasColumnName("ObjAtomId");
            builder.Property(a => a.Name).HasColumnName("AtomName");
            builder.Property(a => a.Priority).HasColumnName("AtomPriority");

            // Теневые свойства
            builder.Property<bool>("Deleted").HasDefaultValue(false);
            // TimeStamp не используется, но колонка в базе осталась. Когда-то давно разработчики договорились, что всё что добавлено после какого-то момента, в базу добавлять с TS = 10
            builder.Property<long>("FakeTimeStamp").HasColumnName("TS").HasDefaultValue(10);

            builder.HasKey(a => a.Id);
        }
    }
}
