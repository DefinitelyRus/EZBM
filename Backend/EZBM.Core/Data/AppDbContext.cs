using Microsoft.EntityFrameworkCore;
using EZBM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EZBM.Core.Data;

/// <summary>
/// Represents the database context for the EZBM application, managing database connections and entity configurations.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the database set for user profiles.
    /// </summary>
    public DbSet<User> User { get; set; }

    /// <summary>
    /// Gets or sets the database set for staff members.
    /// </summary>
    public DbSet<Staff> Staff { get; set; }

    /// <summary>
    /// Gets or sets the database set for customers.
    /// </summary>
    public DbSet<Customer> Customer { get; set; }

    /// <summary>
    /// Gets or sets the database set for system action audit logs.
    /// </summary>
    public DbSet<ActionLog> ActionLog { get; set; }

    /// <summary>
    /// Gets or sets the database set for attendance records.
    /// </summary>
    public DbSet<Attendance> Attendance { get; set; }

    /// <summary>
    /// Gets or sets the database set for payroll transactions.
    /// </summary>
    public DbSet<Payroll> Payroll { get; set; }

    /// <summary>
    /// Gets or sets the database set for inventory items.
    /// </summary>
    public DbSet<Item> Item { get; set; }

    /// <summary>
    /// Gets or sets the database set for item transactions.
    /// </summary>
    public DbSet<ItemTransaction> ItemTransaction { get; set; }

    /// <summary>
    /// Gets or sets the database set for sale transactions.
    /// </summary>
    public DbSet<Sale> Sale { get; set; }

    /// <summary>
    /// Gets or sets the database set for individual sale entries.
    /// </summary>
    public DbSet<SaleEntry> SaleEntry { get; set; }

    /// <summary>
    /// Gets or sets the database set for financial transactions.
    /// </summary>
    public DbSet<Transaction> Transaction { get; set; }


    /// <summary>
    /// Configures the database to be used for this context (SQLite).
    /// </summary>
    /// <param name="optionsBuilder">A builder used to configure database connection options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbManager.DbFilePath}");
    }

    /// <summary>
    /// Configures the model mapping, database schemas, and relationships using Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the database model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Staff>("Staff")
            .HasValue<Customer>("Customer");

        modelBuilder.Entity<User>()
            .Property(u => u.AccessType)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(u => u.Permissions)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        modelBuilder.Entity<User>()
            .Property(u => u.PermissionsAfterExpiry)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        modelBuilder.Entity<Staff>()
            .Property(s => s.PayFrequency)
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

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Staff)
            .WithMany()
            .HasForeignKey("StaffId");

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ParentTransaction)
            .WithMany(t => t.ChildTransactions)
            .HasForeignKey(t => t.ParentTransactionId);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Customer)
            .WithMany(c => c.TransactionHistory)
            .HasForeignKey("CustomerId");

        modelBuilder.Entity<Transaction>()
            .HasMany(t => t.ItemTransactions)
            .WithMany(it => it.Transactions)
            .UsingEntity(j => j.ToTable("TransactionItemTransactions"));

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

    /// <summary>
    /// Saves all changes made in this context to the database, automatically intercepting and auditing modifications.
    /// </summary>
    public override int SaveChanges()
    {
        AuditChanges();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database, automatically intercepting and auditing modifications.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AuditChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AuditChanges()
    {
        List<ActionLog> auditEntries = new();
        List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries = ChangeTracker.Entries()
            .Where(e => (e.State == EntityState.Modified || e.State == EntityState.Deleted) && !(e.Entity is ActionLog))
            .ToList();

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in entries)
        {
            string entityName = entry.Entity.GetType().Name;
            string action = entry.State == EntityState.Modified ? "Edit" : "Delete";
            string entityId = entry.Property("Id").CurrentValue?.ToString() ?? "Unknown";

            string details = $"{action} on {entityName} (ID: {entityId}).";
            if (entry.State == EntityState.Modified)
            {
                List<string> changes = new();
                foreach (Microsoft.EntityFrameworkCore.Metadata.IProperty property in entry.OriginalValues.Properties)
                {
                    object? original = entry.OriginalValues[property];
                    object? current = entry.CurrentValues[property];
                    if (!Equals(original, current))
                    {
                        changes.Add($"{property.Name}: '{original}' -> '{current}'");
                    }
                }
                if (changes.Count > 0)
                {
                    details += " Changes: " + string.Join(", ", changes);
                }
            }

            string operatorUsername = CurrentUserContext.Username ?? "System";
            ActionLog log = new(
                id: Tools.Utils.GenerateEntityId(),
                actionType: action,
                operatorUsername: operatorUsername,
                details: details,
                timestamp: DateTime.UtcNow
            );
            auditEntries.Add(log);
        }

        if (auditEntries.Count > 0)
        {
            ActionLog.AddRange(auditEntries);
        }
    }
}
