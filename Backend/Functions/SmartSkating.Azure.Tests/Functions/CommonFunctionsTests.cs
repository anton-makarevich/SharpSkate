using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Backend.Functions.TestUtils;
using Sanet.SmartSkating.Dto.Models.Responses.Base;
using Xunit;

namespace Sanet.SmartSkating.Backend.Azure.Tests.Functions
{
    public static class CommonFunctionsTests
    {
        public static async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(IAzureFunction sut)
        {
            var actionResult = await sut.Run(Utils.CreateMockRequest(
                    null),
                Substitute.For<IBinder>(),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as ResponseBase;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            Assert.Equal(badRequestStatus, response.ErrorCode);
        }
    }
}