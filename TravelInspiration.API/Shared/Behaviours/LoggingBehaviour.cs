using MediatR;
using MediatR.Pipeline;
using TravelInspiration.API.Shared.Security;

namespace TravelInspiration.API.Shared.Behaviours
{
    public sealed class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        public LoggingBehaviour(ILogger logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting feature exception: {featureFromRequestName}, userId: {userId}", typeof(TRequest).Name, _currentUserService.UserId);
            return Task.CompletedTask;
        }
    }
}
