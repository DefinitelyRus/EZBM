using Microsoft.EntityFrameworkCore;
using EZBM.Core.Entities;
using EZBM.Core.Tools;

namespace EZBM.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Staff> Staff { get; set; }
    // Add others

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbManager.DbFilePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Maps your Enums to strings or ints automatically
        modelBuilder.Entity<Staff>()
            .Property(s => s.PayType)
            .HasConversion<int>();
    }
}