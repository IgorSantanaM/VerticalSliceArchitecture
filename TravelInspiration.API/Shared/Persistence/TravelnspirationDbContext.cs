using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.DomainEvents;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Security;

namespace TravelInspiration.API.Shared.Persistence
{
    public sealed class TravelnspirationDbContext(DbContextOptions<TravelnspirationDbContext> options, IPublisher publisher, ICurrentUserService currentUserService) : DbContext (options)
    {
        public DbSet<Itinerary> Itineraries => Set<Itinerary>();
        public DbSet<Stop> Stops => Set<Stop>();

        private readonly IPublisher _publisher = publisher;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TravelnspirationDbContext).Assembly );
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }

            var domainEvents = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();

            foreach(var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
                domainEvent.IsPublished = true;
            }   
             
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
