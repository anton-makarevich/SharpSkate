using FluentAssertions;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace SmartSkating.Dto.Tests.Services
{
    public class SessionInfoHelperTests
    {
        private readonly SessionInfoHelper _sut = new();

        [Fact]
        public void HubName_Starts_With_LetterS()
        {
            var name = _sut.GetHubNameForSession("123");

            name.Should().StartWith("s");
        }
    }
}