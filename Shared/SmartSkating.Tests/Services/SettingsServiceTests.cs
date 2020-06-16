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
        public void UseGps_IsTrueByDefault()
        {
            _sut.UseGps.Should().BeTrue();
        }
        
        [Fact]
        public void UseBle_IsFalseByDefault()
        {
            _sut.UseBle.Should().BeFalse();
        }
        
        [Fact]
        public void CanInterpolateSectors_IsFalseByDefault()
        {
            _sut.CanInterpolateSectors.Should().BeFalse();
        }
    }
}