using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class SyncHubAuthenticatorFunction
    {
        [FunctionName("SyncHubAuthenticatorFunction")]
        public async Task<IActionResult> Negotiate(
            [HttpTrigger(AuthorizationLevel.Function, "get")]
            HttpRequest req,
            IBinder binder,
            ILogger log)
        {
            var sessionId = req.Query["sessionId"].ToString();
            var connectionInfo = await binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute
                {HubName = sessionId });
            log.LogInformation($"negotiated {connectionInfo}");
            // connectionInfo contains an access key token with a name identifier claim set to the authenticated user
            var response = new SyncHubInfoResponse
            {
                SyncHubInfo = new SyncHubInfoDto
                {
                    Url = connectionInfo.Url,
                    AccessToken = connectionInfo.AccessToken
                }
            };
            return new JsonResult(response);
        }
    }
}
