using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class SyncHubAuthenticatorFunctionTests
    {
        private readonly SyncHubAuthenticatorFunction _sut = new SyncHubAuthenticatorFunction();
        private readonly ILogger _log = Substitute.For<ILogger>();
        private readonly IBinder _binder = Substitute.For<IBinder>();

        [Fact]
        public async Task CallsBinderWithCorrectArguments()
        {
            var request = Utils.CreateMockRequest(queryString: "?sessionId=123");
            await _sut.Negotiate(request, _binder, _log);

            await _binder.Received(1).BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute
                {HubName = "123"});
        }

        [Fact]
        public async Task ReturnsSyncHubInfo_WhenLoginIsSuccessful()
        {
            const string url = "url";
            const string token = "token";
            var request = Utils.CreateMockRequest(queryString: "?sessionId=123");
            _binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute())
                .ReturnsForAnyArgs(new SignalRConnectionInfo
            {
                Url = url,
                AccessToken = token
            });

            var actionResult =  await _sut.Negotiate(request, _binder, _log) as JsonResult;

            actionResult.Should().NotBeNull();
            var response = actionResult?.Value as SyncHubInfoResponse;
            var hubInfo = response?.SyncHubInfo;
            hubInfo.Should().NotBeNull();
            hubInfo?.Url.Should().Be(url);
            hubInfo?.AccessToken.Should().Be(token);
        }
    }
}
