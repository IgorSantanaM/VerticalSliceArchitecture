﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Stops
{
    public sealed class GetStops : ISlice
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapGet("api/itineraries/{itneararyId}/stops",
                (int itneararyId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
                {
                    mediator.Send(new GetStopsQuery(itneararyId), cancellationToken);

                }).RequireAuthorization();
        }
    }

    public sealed class GetStopsQuery(int itineraryId) : IRequest<IResult>
    {
        public int ItineraryId { get; } = itineraryId;
    }

    public sealed class GetStopsHandler(TravelnspirationDbContext dbContext,
        IMapper mapper) : IRequestHandler<GetStopsQuery, IResult>
    {
        private readonly TravelnspirationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;

        public async Task<IResult> Handle(GetStopsQuery request, CancellationToken cancellationToken)
        {
            var itinerary = await _dbContext.Itineraries
                    .Include(i => i.Stops)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == request.ItineraryId, cancellationToken);

            if (itinerary is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(_mapper.Map<IEnumerable<StopDto>>(itinerary.Stops));
        }
    }

    public sealed class StopDto
    {
        public required int Id { get; set; } 
        public required string Name { get; set; } 
        public Uri? ImageUri { get; set; } 
        public required int ItineraryId{ get; set; } 
    }

    public sealed class StopMapProfile : Profile
    {
        public StopMapProfile()
        {
            CreateMap<Stop, StopDto>().ReverseMap();
        }
    }
}
