using System.Data.Entity;
using QBProduction.Web.Models;

namespace QBProduction.Web.Data
{
    public class QBProductionContext : DbContext
    {
        public QBProductionContext() : base("name=DB")
        {
            // Disable automatic migrations for production safety
            Database.SetInitializer<QBProductionContext>(null);
        }

        public DbSet<Boms> Boms { get; set; }
        public DbSet<BomItems> BomItems { get; set; }
        public DbSet<BomRun> BomRuns { get; set; }
        public DbSet<BomRunItems> BomRunItems { get; set; }
        public DbSet<BomSettings> BomSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Boms
            modelBuilder.Entity<Boms>()
                .HasMany(b => b.BomItems)
                .WithOptional(bi => bi.bomid)
                .HasForeignKey(bi => bi.BomsId);

            // Configure BomRun
            modelBuilder.Entity<BomRun>()
                .HasMany(br => br.BomRunItems)
                .WithOptional(bri => bri.bomid)
                .HasForeignKey(bri => bri.BomRunId)
                .WillCascadeOnDelete(true);

            // Configure unique constraint for bomrunref
            modelBuilder.Entity<BomRun>()
                .HasIndex(br => br.bomrunref)
                .IsUnique();
        }
    }
}
