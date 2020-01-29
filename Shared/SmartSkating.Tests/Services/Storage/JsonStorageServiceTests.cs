using FluentAssertions;
using Sanet.SmartSkating.Services.Storage;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Storage
{
    public class JsonStorageServiceTests
    {
        [Fact]
        public void DefaultErrorMessageIsEmpty()
        {
            var sut = new JsonStorageService();

            sut.ErrorMessage.Should().BeEmpty();
        }
    }
}