using Microsoft.EntityFrameworkCore;
using EZBM.Core.Entities;

namespace EZBM.Core.Data;

/// <summary>
/// Represents the database context for the EZBM application, managing database connections and entity configurations.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the database set for staff members.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Staff> Staff { get; set; }

    /// <summary>
    /// Gets or sets the database set for attendance records.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Attendance> Attendance { get; set; }

    /// <summary>
    /// Gets or sets the database set for payroll transactions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Payroll> Payroll { get; set; }

    /// <summary>
    /// Gets or sets the database set for inventory items.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Item> Item { get; set; }

    /// <summary>
    /// Gets or sets the database set for item transactions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<ItemTransaction> ItemTransaction { get; set; }

    /// <summary>
    /// Gets or sets the database set for sale transactions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Sale> Sale { get; set; }

    /// <summary>
    /// Gets or sets the database set for individual sale entries.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<SaleEntry> SaleEntry { get; set; }

    /// <summary>
    /// Gets or sets the database set for financial transactions.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public DbSet<Transaction> Transaction { get; set; }

    /// <summary>
    /// Configures the database to be used for this context (SQLite).
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="optionsBuilder">A builder used to configure database connection options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbManager.DbFilePath}");
    }

    /// <summary>
    /// Configures the model mapping, database schemas, and relationships using Fluent API.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the database model.</param>
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
