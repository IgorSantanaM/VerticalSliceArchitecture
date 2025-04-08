using TravelInspiration.API.Features.Stops;
using TravelInspiration.API.Shared.Domain.Entities;
using TravelInspiration.API.Shared.Domain.Events;

namespace TravelInpiration.Api.UnitTests.Shared.Domain
{
    public class StopTests : IDisposable
    {
        private readonly Stop _stopEntity;
        private readonly CreateStop.CreateStopCommand _createStopCommand;
        public StopTests()
        {
            _stopEntity = new Stop("Test Stop");
            _createStopCommand = new CreateStop.CreateStopCommand(42, "Test Stop", null);
        }

        [Fact]
        public void WhenExecutingHandleCreateCommand_WithItineraryID_StopItineraryIdMustMatch()
        {
            // ARRANGE
            //nothing to see here

            // ACT
            _stopEntity.HandleCreateCommand(_createStopCommand);

            // ASSERT
            Assert.Equal(_createStopCommand.ItineraryId, _stopEntity.ItineraryId);
        }

        [Fact]
        public void WhenExecutingHandleCreateCommand_WithValidInput_OneStopCreatedEventMustBeAdded()
        {
            // ARRANGE
            // nothing to see here

            // ACT
            _stopEntity.HandleCreateCommand(_createStopCommand);

            // ASSERT
            Assert.Single(_stopEntity.DomainEvents);
            Assert.IsType<StopCreatedEvent>(_stopEntity.DomainEvents.First());

        }
        
        public void Dispose()
        {
            // no code
        }
    }
}
