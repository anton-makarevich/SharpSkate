using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class DeviceSaverFunctionTests
    {
        private readonly DeviceSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly DeviceDto _deviceStub = new DeviceDto
        {
            AccountId = "accountId",
            Id = "deviceId",
            Manufacturer = "some",
            Model = "model",
            OsName = "os",
            OsVersion = "1"
        };

        public DeviceSaverFunctionTests()
        {
            _dataService = Substitute.For<IDataService>();
            _sut = new DeviceSaverFunction();
            _sut.SetService(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveDevice()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _deviceStub),
                Substitute.For<ILogger>());

            await _dataService.Received().SaveDeviceAsync(Arg.Any<DeviceDto>());
        }

        [Fact]
        public async Task RunningFunctionReturnsTrue_WhenSaveIsSuccessful()
        {
            _dataService.SaveDeviceAsync(_deviceStub)
                .ReturnsForAnyArgs(Task.FromResult(true));

            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _deviceStub),
                Substitute.For<ILogger>()) as JsonResult;

            Assert.NotNull(actionResult);
            var response = actionResult.Value as BooleanResponse;

            response.Should().NotBeNull();
            response?.Result.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnsServiceErrorMessage_WhenSavingIsUnsuccessful()
        {
            const string errorMessage = "some error";
            _dataService.ErrorMessage.Returns(errorMessage);
            _dataService.SaveDeviceAsync(_deviceStub)
                .ReturnsForAnyArgs(Task.FromResult(false));

            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _deviceStub),
                Substitute.For<ILogger>()) as JsonResult;

            Assert.NotNull(actionResult);
            var response = actionResult.Value as BooleanResponse;
            Assert.NotNull(response);
            response.Message.Should().Contain(errorMessage);
        }

        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            await CommonFunctionsTests.RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(_sut);
        }
    }
}
