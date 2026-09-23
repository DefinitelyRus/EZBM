using Backend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBatch> ProductBatches { get; set; }
    public DbSet<ProductTransaction> ProductTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /*
        By default, deleting a batch also deletes every transaction tied to it.
        This is a result of DeleteBehavior.Cascade. So, we enforce a rule where
        when a batch is deleted, the product transactions tied to it will not be
        deleted. Instead, their batch property will simply be set to null.
        */
        builder.Entity<ProductTransaction>()
            .HasOne(t => t.Batch)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }
}