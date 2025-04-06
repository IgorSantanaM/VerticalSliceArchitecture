using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Domain.Events;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Stops
{
    public sealed class UpdateStop : ISlice
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPut("" +
                "api/itineraries/{itineraryId}/stops/{stopId}",
                (int itineraryId, int stopId, UpdateStopCommand updateStopCommand, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    updateStopCommand.ItineraryId = itineraryId;
                    updateStopCommand.StopId = stopId;
                });
        }

        public sealed class UpdateStopCommand : IRequest<IResult>
        {
            public int ItineraryId { get; set; }
            public int StopId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? ImageUri { get; set; }
            public bool? Suggested { get; set; }
        }

        public sealed class UpdateStopCommandValidator : AbstractValidator<UpdateStopCommand>
        {
            public UpdateStopCommandValidator()
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

        public sealed class UpdateStopCommandHandler : IRequestHandler<UpdateStopCommand, IResult>
        {
            private readonly TravelnspirationDbContext _dbContext;
            private readonly IMapper _mapper;

            public UpdateStopCommandHandler(TravelnspirationDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<IResult> Handle(UpdateStopCommand request, CancellationToken cancellationToken)
            {
                var stopToUpdate = await _dbContext.Stops
                    .FirstOrDefaultAsync(s => s.Id == request.StopId
                        && s.ItineraryId == request.ItineraryId, cancellationToken);

                if (stopToUpdate is null)
                {
                    return Results.NotFound();
                }

                stopToUpdate.HandleUpdateCommand(request);

                await _dbContext.SaveChangesAsync(cancellationToken);

                var stopDto = _mapper.Map<StopDto>(stopToUpdate);

                return Results.Ok(stopDto);
            }
        }
        public sealed class StopDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool? Suggested { get; set; }
            public string? ImageUri { get; set; }
            public required int ItineraryId { get; set; }
        }

        public sealed class StopMapProfileAfterUpdate : Profile
        {
            public StopMapProfileAfterUpdate()
            {
                CreateMap<Stop, StopDto>().ReverseMap();
            }
        }
    }
}
