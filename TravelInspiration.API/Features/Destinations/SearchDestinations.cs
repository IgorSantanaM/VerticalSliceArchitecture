using Polly;
using TravelInspiration.API.Shared.Networking;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Destinations;
public sealed class SearchDestinations : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/destinations", async (string? searchFor,
            ILoggerFactory logger,
            IDestinationSearchApiClient destinationApiClient,
            CancellationToken cancellationToken) =>
        {
            logger.CreateLogger("EndpointHandlers").LogInformation("SearchDestinations feature called.");

            var resultFromApi = await destinationApiClient
                .GetDestinationsAsync(searchFor, cancellationToken);

            var result = resultFromApi.Select(d => new
            {
                d.Name,
                d.Description,
                d.ImageUri
            });

            return Results.Ok(result);
        });
    }
}
