using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.Entities;

namespace TravelInspiration.API.Shared.Persistence
{
    public sealed class TravelnspirationDbContext(DbContextOptions<TravelnspirationDbContext> options) : DbContext (options)
    {
        public DbSet<Itinerary> Itineraries => Set<Itinerary>();
        public DbSet<Stop> Stops => Set<Stop>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TravelnspirationDbContext).Assembly );
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "TODO";
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "TODO";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "TODO";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
