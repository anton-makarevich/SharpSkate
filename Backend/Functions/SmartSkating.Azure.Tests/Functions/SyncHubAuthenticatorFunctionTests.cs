using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sanet.SmartSkating.Backend.Functions;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public class SyncHubAuthenticatorFunctionTests
    {
        private readonly SyncHubAuthenticatorFunction _sut = new SyncHubAuthenticatorFunction();
        private readonly ILogger _log = Substitute.For<ILogger>();
        private readonly IBinder _binder = Substitute.For<IBinder>();
        private const string SessionId = "123";
        private readonly HttpRequest _request = Utils.CreateMockRequest();

        [Fact]
        public async Task CallsBinderWithCorrectArguments()
        {
            _binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute())
                .ReturnsForAnyArgs(new SignalRConnectionInfo());

            await _sut.Negotiate(_request,SessionId, _binder, _log);

            await _binder.Received(1).BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute
                {HubName = SessionId});
        }

        [Fact]
        public async Task ReturnsSyncHubInfo_WhenNegotiationIsSuccessful()
        {
            const string url = "url";
            const string token = "token";
            _binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute())
                .ReturnsForAnyArgs(new SignalRConnectionInfo
            {
                Url = url,
                AccessToken = token
            });

            var actionResult =  await _sut.Negotiate(_request,SessionId, _binder, _log) as JsonResult;

            actionResult.Should().NotBeNull();
            var hubInfo = actionResult?.Value as SignalRConnectionInfo;
            hubInfo.Should().NotBeNull();
            (hubInfo?.Url).Should().Be(url);
            (hubInfo?.AccessToken).Should().Be(token);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenNegotiationIsNotSuccessful()
        {
            const string errorMessage = "Something is wrong";
            _binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute())
                .ThrowsForAnyArgs(new Exception(errorMessage));

            var response = await _sut.Negotiate(_request,SessionId, _binder, _log) as NotFoundResult;

            response.Should().NotBeNull();
        }
    }
}
