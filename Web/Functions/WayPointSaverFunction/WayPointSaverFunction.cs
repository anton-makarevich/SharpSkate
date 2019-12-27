using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sanet.SmartSkating.Azure.Services;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Web.Functions
{
    public class WayPointSaverFunction
    {
        private IDataService _dataService;

        public void SetService(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        [FunctionName("WayPointSaverFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "waypoints")] HttpRequest request,
            ILogger log)
        {
            if (_dataService == null)
                SetService(new AzureTablesDataService(log));

            var responseObject = new SaveWayPointsResponse();
            responseObject.SyncedWayPointsIds = new List<string>();
            var requestData = await new StreamReader(request.Body).ReadToEndAsync();
            var requestObject = JsonConvert.DeserializeObject<List<WayPointDto>>(requestData);
            
            if (requestObject == null)
            {
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
                responseObject.Message = "Invalid request data";
            }
            else
            {
                responseObject.ErrorCode = (int)HttpStatusCode.OK;
                foreach (var wayPoint in requestObject)
                {
                    if (await _dataService.SaveWayPointAsync(wayPoint))
                        responseObject.SyncedWayPointsIds.Add(wayPoint.Id);
                }
                
                responseObject.Message = _dataService.ErrorMessage;
            }
            return new JsonResult(responseObject);
        }
    }
}