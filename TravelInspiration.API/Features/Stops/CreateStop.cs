﻿using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Domain.Events;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Security;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Stops
{
    public class CreateStop : ISlice
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPost(
                "api/itineraries/{itinerartyId}/stops",
                (int itinerartyId,
                CreateStopCommand createStopCommand,
                IMediator mediator,
                CancellationToken cancellationToken) =>
                {
                    createStopCommand.ItineraryId = itinerartyId;
                    return mediator.Send(createStopCommand);
                }).RequireAuthorization(AuthorizationPolicies.HasWriteActionPolicy);
        }
        public sealed class CreateStopCommand(int itineraryId,
            string name,
            string? imageUri) : IRequest<IResult>
        {
            public int ItineraryId { get; set; } = itineraryId;
            public string Name { get; set; } = name;
            public string? ImageUri { get; set; } = imageUri;

        }
        public sealed class CreateStopCommandValidator : AbstractValidator<CreateStopCommand>
        {
            public CreateStopCommandValidator()
            {
                RuleFor(n => n.Name)
                    .NotEmpty()
                    .MaximumLength(200)
                    .WithMessage("Name must be less than 200 characters");
                RuleFor(i => i.ImageUri)
                    .Must(ImageUri => Uri.TryCreate(ImageUri ?? "", UriKind.Absolute, out _))
                    .When(i => !string.IsNullOrEmpty(i.ImageUri))
                    .WithMessage("ImageUri must be a valid URI");
            }
        }
        public sealed class CreateStopCommandHandler(TravelnspirationDbContext dbContext, IMapper mapper) : IRequestHandler<CreateStopCommand, IResult>
        {
            private readonly TravelnspirationDbContext _dbContext = dbContext;
            private readonly IMapper _mapper = mapper;

            public async Task<IResult> Handle(CreateStopCommand request, CancellationToken cancellationToken)
            {
                if (!await _dbContext.Itineraries
                    .AnyAsync(i => i.Id == request.ItineraryId, cancellationToken))
                {
                    return Results.NotFound();
                }
                var stopEntity = new Stop(request.Name);
                stopEntity.HandleCreateCommand(request);

                _dbContext.Stops.Add(stopEntity);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Results.Created(
                    $"api/intineraries/{stopEntity.ItineraryId}/stops/{stopEntity
                    .Id}",
                    _mapper.Map<StopDto>(stopEntity));

            }
        }
        public sealed class StopDto
        {
            public required int Id { get; set; }
            public required string Name { get; set; }
            public bool? Suggested { get; set; }
            public Uri? ImageUri { get; set; }
            public required int ItineraryId { get; set; }

        }
        public sealed class StopMapProfileAfterCreation : Profile
        {
            public StopMapProfileAfterCreation()
            {
                CreateMap<Stop, StopDto>().ReverseMap();
            }
        }

        public sealed class SuggestedStopCreatedEventHandler(ILogger logger, TravelnspirationDbContext dbContext) : INotificationHandler<StopCreatedEvent>
        {
            public readonly ILogger _logger = logger;
            public readonly TravelnspirationDbContext _dbContext = dbContext;

            public Task Handle(StopCreatedEvent notification, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Listener {listener} to domain event {domainEvent} triggered.",
                    GetType().Name, notification.GetType().Name);

                var incomingStop = notification.Stop;

                // some AI to generate suggested stop

                var stop = new Stop($"AI-ified stop based on {incomingStop.Name}")
                {
                    ItineraryId = incomingStop.ItineraryId,
                    ImageUri = new Uri("https://herebeimages.ciom/aigeneratedimage.jpg"),
                    Suggested = true
                };

                _dbContext.Stops.Add(stop);
                return Task.CompletedTask;
            }
        }
    }
}
