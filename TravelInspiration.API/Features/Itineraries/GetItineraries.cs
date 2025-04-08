using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Itineraries;
public sealed class GetItineraries : ISlice
{
    private AuthorizationPolicy _hasGetItinerariesPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("feature", "get-itineraries")
        .Build();
    ///Searchs up in the provided request token if it has a key feature with the value get-itineraries

    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "api/itineraries",
            (string? searchFor,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                return mediator.Send(new GetItinerariesQuery(searchFor), cancellationToken);
            })  .RequireAuthorization(_hasGetItinerariesPolicy);
    }

    public sealed class GetItinerariesQuery(string? searchFor) : IRequest<IResult>
    {
        public string? SearchFor { get; } = searchFor;

    }

    public sealed class ItineraryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required string UserId { get; set; }

    }

    public sealed class IteneraryMapProfile : Profile
    {
        public IteneraryMapProfile()
        {
            CreateMap<Itinerary, ItineraryDto>().ReverseMap();
        }
    }

    public sealed class GetItinerariesHandler(TravelnspirationDbContext dbContext,
        IMapper mapper, ILogger<GetItinerariesHandler> logger) : IRequestHandler<GetItinerariesQuery, IResult>
    {

        private readonly TravelnspirationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetItinerariesHandler> _logger = logger;

        public async Task<IResult> Handle(GetItinerariesQuery request, CancellationToken cancellationToken)
        {

            var itineraries = await
                dbContext.Itineraries.Where(i => request.SearchFor == null ||
                i.Name.Contains(request.SearchFor) || (i.Description != null && i.Description.Contains(request.SearchFor)))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(_mapper.Map<IEnumerable<ItineraryDto>>(itineraries));

        }
    }
}
