namespace TravelInspiration.API.Shared.Domain.DomainEvents
{
    public interface IHasDomainEvent
    {
        IList<DomainEvent> DomainEvents { get; }
    } 
}
