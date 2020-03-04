using System;
using System.Threading.Tasks;
using FluentAssertions;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Services.Storage;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Storage
{
    public class JsonStorageServiceTests
    {
        private readonly JsonStorageService _sut;

        public JsonStorageServiceTests()
        {
            _sut = new JsonStorageService();
        }
        
        [Fact]
        public void DefaultErrorMessageIsEmpty()
        {
            _sut.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task ThrowNotImplemented_ForSaveDeviceAsync()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => _sut.SaveDeviceAsync(new DeviceDto()));
        }
    }
}