using MediatR;
using TravelInspiration.API.Shared.Domain.Events;

namespace TravelInspiration.API.Shared.DomainEventHandlers
{
    public sealed class SuggestStopStopUpdatedEventHandler(
            ILogger<SuggestStopStopUpdatedEventHandler> logger)
            : INotificationHandler<StopUpdatedEvent>
    {
        private readonly ILogger<SuggestStopStopUpdatedEventHandler> _logger = logger;

        public Task Handle(StopUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listener: {Listener}, to Domain Event: {domaiEvent} triggered",
                GetType().Name,
                notification.GetType().Name);


            return Task.CompletedTask;
        }
    }
}
