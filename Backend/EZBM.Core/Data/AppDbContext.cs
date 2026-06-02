using Microsoft.EntityFrameworkCore;
using EZBM.Core.Entities;

namespace EZBM.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Attendance> Attendance { get; set; }
    public DbSet<Payroll> Payroll { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<ItemTransaction> ItemTransaction { get; set; }
    public DbSet<Sale> Sale { get; set; }
    public DbSet<SaleEntry> SaleEntry { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbManager.DbFilePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map Enums to strings automatically
        modelBuilder.Entity<Staff>()
            .Property(s => s.PayType)
            .HasConversion<string>();
        
        modelBuilder.Entity<ItemTransaction>()
            .Property(s => s.TransactionType)
            .HasConversion<string>();
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.TransactionType)
            .HasConversion<string>();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.PaymentMethod)
            .HasConversion<string>();

        // Shadow object-type properties as foreign keys (IDs)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Staff)
            .WithMany()
            .HasForeignKey("StaffId");

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Staff)
            .WithMany()
            .HasForeignKey("StaffId");

        modelBuilder.Entity<ItemTransaction>()
            .HasOne(it => it.Item)
            .WithMany()
            .HasForeignKey("ItemId");

        modelBuilder.Entity<ItemTransaction>()
            .HasOne(it => it.Staff)
            .WithMany()
            .HasForeignKey("StaffId");

        modelBuilder.Entity<ItemTransaction>()
            .HasOne(it => it.SaleEntry)
            .WithMany()
            .HasForeignKey("SaleEntryId");

        modelBuilder.Entity<SaleEntry>()
            .HasOne(se => se.Sale)
            .WithMany()
            .HasForeignKey("SaleId");

        modelBuilder.Entity<SaleEntry>()
            .HasOne(se => se.Item)
            .WithMany()
            .HasForeignKey("ItemId");
    }
}
