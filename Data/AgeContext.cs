using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Age.Data
{
    public class AgeContext : DbContext
    {
        public static string ConnS;
        public AgeContext()
        {
        }

        public AgeContext(DbContextOptions<AgeContext> options) : base(options)
        {
            var sqlServerOptionsExtension = options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtension != null)
            {
                ConnS = sqlServerOptionsExtension.ConnectionString;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnS);
        }

        public DbSet<AgeGetNumber> AgeGetNumber { get; set; }
        public DbSet<AgeYearSet> AgeYearSet { get; set; }
        public DbSet<AgeYearDaySet> AgeYearDaySet { get; set; }
        // public DbSet<VPCategory> VPCategory { get; set; }
        // public DbSet<VPType> VPType { get; set; }
        // public DbSet<A2E> A2E { get; set; }
        // public DbSet<NHIQP701> NHIQP701 { get; set; }
        // public DbSet<chklist> chklist { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }

}