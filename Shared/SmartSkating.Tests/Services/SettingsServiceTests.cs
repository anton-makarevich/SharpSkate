using FluentAssertions;
using Sanet.SmartSkating.Services;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services
{
    public class SettingsServiceTests
    {
        private readonly SettingsService _sut;

        public SettingsServiceTests()
        {
            _sut = new SettingsService();
        }


        [Fact]
        public void UseGpsIsTrueByDefault()
        {
            _sut.UseGps.Should().BeTrue();
        }
        
        [Fact]
        public void UseBleIsFalseByDefault()
        {
            _sut.UseBle.Should().BeFalse();
        }
    }
}