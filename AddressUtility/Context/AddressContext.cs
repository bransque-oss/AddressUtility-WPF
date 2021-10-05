using AddressUtility.Context.Configurations;
using AddressUtility.Context.DataSeeding;
using AddressUtility.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace AddressUtility.Context
{
    public class AddressContext : DbContext
    {
        public DbSet<AddressObject> AddressObjects { get; set; }
        public DbSet<Atom> Atoms { get; set; }
        public DbSet<AddrType> AddrTypes { get; set; }
        public DbSet<Region> Regions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                            .SetBasePath(AppContext.BaseDirectory)
                                            .AddJsonFile("appsettings.json")
                                            .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("AddressDB"), x => x.UseNetTopologySuite());
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AddressObjectConfiguration());
            builder.ApplyConfiguration(new AddrTypeConfiguration());
            builder.ApplyConfiguration(new AtomConfiguration());
            builder.ApplyConfiguration(new RegionConfiguration());

            // Наполнение тестовыми данными
            SampleData sample = new();
            sample.Seed(builder);
        }
    }
}
