using MediatR;
using MediatR.Pipeline;

namespace TravelInspiration.API.Shared.Behaviours
{
    public sealed class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;

        public LoggingBehaviour(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting feature exception: {featureFromRequestName}", typeof(TRequest).Name);
            return Task.CompletedTask;
        }
    }
}
