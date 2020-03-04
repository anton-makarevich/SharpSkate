using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sanet.SmartSkating.Backend.Azure;
using Sanet.SmartSkating.Backend.Azure.Services;
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class DeviceSaverFunction: IAzureFunction
    {
        private IDataService? _dataService;

        public void SetService(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();
        
        [FunctionName("DeviceSaverFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, 
                "post",
                Route = ApiNames.DevicesResource.Route)] HttpRequest request,
            ILogger logger)
        {
            if (_dataService == null)
                SetService(new AzureTablesDataService(logger));

            var responseObject = new BooleanResponse();
            var requestData = await new StreamReader(request.Body).ReadToEndAsync();
            
            var requestObject = JsonConvert.DeserializeObject<DeviceDto>(requestData);
            
            if (requestObject == null)
            {
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                responseObject.ErrorCode = (int)HttpStatusCode.OK;
                
                if (_dataService != null)
                    responseObject.Result = await _dataService.SaveDeviceAsync(requestObject);
                
                if (!string.IsNullOrEmpty(_dataService?.ErrorMessage)) 
                    _errorMessageBuilder.AppendLine(_dataService.ErrorMessage);
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            if (responseObject.Message.Contains(Constants.DateTimeValidationErrorMessage))
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
            return new JsonResult(responseObject);
        }
    }
}