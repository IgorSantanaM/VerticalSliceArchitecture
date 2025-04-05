using System.Diagnostics;
using MediatR;
using TravelInspiration.API.Shared.Metrics;

namespace TravelInspiration.API.Shared.Behaviours
{
    public sealed class HandlePerformanceMetricBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly HandlePerformanceMetric _handlePerformanceMetric;
        private readonly Stopwatch _timer = new();

        public HandlePerformanceMetricBehaviour(HandlePerformanceMetric handlePerformanceMetric)
        {
            _handlePerformanceMetric = handlePerformanceMetric;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            _timer.Start();
            var respose = await next();
            _timer.Stop();

            _handlePerformanceMetric.MiliSecondsElapsed(_timer.ElapsedMilliseconds);

            return respose;
        }
    }
}
