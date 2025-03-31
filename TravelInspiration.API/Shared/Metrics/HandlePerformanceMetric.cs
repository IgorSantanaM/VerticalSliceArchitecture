using System.Diagnostics.Metrics;

namespace TravelInspiration.API.Shared.Metrics
{
    public sealed class HandlePerformanceMetric
    {

        private readonly Counter<long> _miliSecondsElapsed;
        public HandlePerformanceMetric(IMeterFactory meterFactory)
        {

            var meter = meterFactory.Create("TravelInspiration.API");
            _miliSecondsElapsed = meter.CreateCounter<long>("travelinspiration.api.requesthandler.milisecondselapsed");
        }
        public void MiliSecondsElapsed(long milliSecondsElapsed)
            => _miliSecondsElapsed.Add(milliSecondsElapsed);
    }
}
