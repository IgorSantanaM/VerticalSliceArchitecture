using Microsoft.AspNetCore.Http;
using Moq;
using System.Net.WebSockets;
using System.Security.Claims;
using TravelInspiration.API.Shared.Security;

namespace TravelInpiration.Api.UnitTests.Shared.Security
{
    public sealed class CurrentUserServiceTests
    {
        [Fact]
        public void WhenGettingUser_WithNameIdentifierClaimInContext_NameIdentifierMustBeReturned()
        {
            // ARRANGE
            var httpContextAccessor = new Mock<HttpContextAccessor>();
            var identity = new ClaimsIdentity(
                new List<Claim>() { new(ClaimTypes.NameIdentifier, "igors") },
                "Test", ClaimTypes.NameIdentifier, ClaimTypes.Role);

            var contextUser = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            httpContextAccessor.Setup(x => x.HttpContext)
                .Returns(httpContext);

            var currentUserService = new CurrentUserService(httpContextAccessor.Object);

            // ACT
            var userId = currentUserService.UserId;
            // ASSERT
            Assert.Equal(identity.Name, userId);
        }
    }
}
