using System;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace SmartSkating.Dto.Tests.Services
{
    public class DateProviderTests
    {
        [Fact]
        public void Now_Is_Almost_Now()
        {
            var sut = new DateProvider();

            sut.Now().Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0,0,0,0,100));
        }
    }
}
