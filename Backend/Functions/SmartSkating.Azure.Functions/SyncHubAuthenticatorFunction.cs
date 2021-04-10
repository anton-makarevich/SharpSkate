using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class SyncHubAuthenticatorFunction
    {
        private readonly ISessionInfoHelper _sessionHelper;

        public SyncHubAuthenticatorFunction(ISessionInfoHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
        }
        
        [FunctionName("SyncHubAuthenticatorFunction")]
        public async Task<IActionResult> Negotiate(
            [HttpTrigger(AuthorizationLevel.Function, "post",
                Route = "{sessionId}/negotiate")]
            HttpRequest request,
            string sessionId,
            IBinder binder,
            ILogger log)
        {
            try
            {
                var connectionInfo = await binder
                    .BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute
                    {HubName = _sessionHelper.GetHubNameForSession(sessionId) });
                log.LogInformation($"negotiated {connectionInfo}");
                // connectionInfo contains an access key token with a name identifier claim set to the authenticated user
                return new JsonResult(connectionInfo);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return new NotFoundResult();
            }
        }
    }
}
