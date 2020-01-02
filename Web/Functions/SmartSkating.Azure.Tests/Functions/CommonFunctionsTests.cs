using System.Net;
using System.Threading.Tasks;
using FunctionTestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models.Responses;
using Xunit;

namespace Sanet.SmartSkating.Azure.Tests.Functions
{
    public static class CommonFunctionsTests
    {
        public static async Task RunningFunctionWithoutProperRequestReturnsBadRequestErrorCode(IAzureFunction sut)
        {
            var actionResult = await sut.Run(Utils.CreateMockRequest(
                    null),
                Substitute.For<ILogger>()) as JsonResult;
        
            Assert.NotNull(actionResult);
            var response = actionResult.Value as SaveEntitiesResponse;
        
            Assert.NotNull(response);
            const int badRequestStatus = (int) HttpStatusCode.BadRequest;
            Assert.Equal(badRequestStatus, response.ErrorCode);
        }
    }
}