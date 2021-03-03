using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class BleScanSaverFunctionTests
    {
        private readonly BleScanSaverFunction _sut;
        private readonly IDataService _dataService;
        private readonly List<BleScanResultDto> _scansStub = new List<BleScanResultDto>();

        private BleScanResultDto GetBleScanStub(int id)
        {
            return new BleScanResultDto
            {
                Id = id.ToString(),
                SessionId = "0",
                DeviceAddress = "6",
                Rssi = -87+id,
                Time = DateTime.Now
            };
        }

        public BleScanSaverFunctionTests()
        {
            foreach (var i in new[]{0,1})
            {
                _scansStub.Add(GetBleScanStub(i));
            }
            _dataService = Substitute.For<IDataService>();
            _sut = new BleScanSaverFunction(_dataService);
        }

        [Fact]
        public async Task RunningFunctionCallsSaveBleScanForEveryItem()
        {
            await _sut.Run(Utils.CreateMockRequest(
                    _scansStub),
                Substitute.For<ILogger>());

            await _dataService.Received(2).SaveBleScanAsync(Arg.Any<BleScanResultDto>());
        }

        [Fact]
        public async Task RunningFunctionReturnsListOfSavedBleScsnsIds()
        {
            _dataService.SaveBleScanAsync(Arg.Any<BleScanResultDto>())
                .ReturnsForAnyArgs(Task.FromResult(true));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _scansStub),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response?.SyncedIds);
            Assert.Equal(200, response.ErrorCode);
            Assert.Equal(2, response.SyncedIds.Count);
            Assert.Equal(_scansStub.First().Id,response.SyncedIds.First());
            Assert.Equal(_scansStub.Last().Id,response.SyncedIds.Last());
        }
        
        [Fact]
        public async Task ReturnsServiceErrorMessage_WhenSavingIsUnsuccessful()
        {
            const string errorMessage = "some error";
            _dataService.ErrorMessage.Returns(errorMessage);
            _dataService.SaveBleScanAsync(Arg.Any<BleScanResultDto>())
                .ReturnsForAnyArgs(Task.FromResult(false));
        
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _scansStub),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
            Assert.NotNull(response);
            response.Message.Should().Contain(errorMessage);
        }
        
        [Fact]
        public async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode()
        {
            await CommonFunctionsTests.RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(_sut);
        }
        
        [Fact]
        public async Task ReturnsBadRequestWithAMessage_WhenTimeIsLessThanMinValueForEveryScan()
        {
            foreach (var bleScanResultDto in _scansStub)
            {
                bleScanResultDto.Time = DateTime.MinValue;
            }
            _dataService.SaveBleScanAsync(Arg.Any<BleScanResultDto>())
                .ReturnsForAnyArgs(Task.FromResult(true));
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _scansStub),
                Substitute.For<ILogger>()) as JsonResult;
            
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            response.ErrorCode.Should().Be(badRequestStatus);
            response.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ReturnsOkWithAMessage_WhenTimeIsLessThanMinValueForNotEveryScan()
        {
            _scansStub.First().Time = DateTime.MinValue;
            _dataService.SaveBleScanAsync(Arg.Any<BleScanResultDto>())
                .ReturnsForAnyArgs(Task.FromResult(true));
            
            var actionResult = await _sut.Run(Utils.CreateMockRequest(
                    _scansStub),
                Substitute.For<ILogger>()) as JsonResult;
            
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int okStatus = (int) HttpStatusCode.OK;
            response.ErrorCode.Should().Be(okStatus);
            response.Message.Should().NotBeNullOrEmpty();
        }
    }
}