using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "huydz"; // TODO: This will be replaced by Identity User
                        break;
                    case EntityState.Modified:
                            entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "huydz"; // TODO: This will be replaced by Identity User
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.UserName)
                    .HasMaxLength(70);
                entity.Property(e => e.FirstName)
                    .HasMaxLength(255);
                entity.Property(e => e.LastName)
                    .HasMaxLength(255);
                entity.Property(e => e.Email)
                    .HasMaxLength(255);
                entity.Property(e => e.AddressLine)
                    .HasMaxLength(255);
                entity.Property(e => e.Country)
                    .HasMaxLength(255);
                entity.Property(e => e.State)
                    .HasMaxLength(255);
                entity.Property(e => e.ZipCode)
                    .HasMaxLength(10);
                entity.Property(e => e.CardName)
                    .HasMaxLength(255);
                entity.Property(e => e.CardNumber)
                    .HasMaxLength(20);
                entity.Property(e => e.Expiration)
                    .HasMaxLength(20);
                entity.Property(e => e.CVV)
                    .HasMaxLength(10);
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(70);
                entity.Property(e => e.LastModifiedBy)
                    .HasMaxLength(70);
                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}
