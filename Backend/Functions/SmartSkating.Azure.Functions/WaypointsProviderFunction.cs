using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Sanet.SmartSkating.Backend.Azure;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class WaypointsProviderFunction:IAzureFunction
    {
        private readonly IDataService _dataService;
        
        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();

        public WaypointsProviderFunction(IDataService dataService)
        {
            _dataService = dataService;
        }

        [FunctionName("WaypointsProviderFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,
                "get",
                Route = ApiNames.WayPointsResource.Route)]
            HttpRequest request, IBinder _, ILogger logger)
        {
            var sessionId = request.Query["sessionId"].ToString();
            
            var responseObject = new GetWaypointsResponse();
            
            if (string.IsNullOrEmpty(sessionId))
            {
                responseObject.ErrorCode = StatusCodes.Status400BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                var waypoints = await _dataService.GetWayPointForSessionAsync(sessionId);
                
                responseObject.Waypoints = waypoints;
                responseObject.ErrorCode = StatusCodes.Status200OK;
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            if (responseObject.ErrorCode != 200)
                logger.LogInformation(responseObject.Message);
            return new JsonResult(responseObject);
        }
    }
}