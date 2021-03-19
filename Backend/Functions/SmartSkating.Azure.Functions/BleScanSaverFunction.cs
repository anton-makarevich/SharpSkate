using System.Collections.Generic;
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
using Sanet.SmartSkating.Dto;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class BleScanSaverFunction: IAzureFunction
    {
        private readonly IDataService _dataService;

        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();

        public BleScanSaverFunction(IDataService dataService)
        {
            _dataService = dataService;
        }

        [FunctionName("BleScanSaverFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, 
                "post",
                Route = ApiNames.BleScansResource.Route)] HttpRequest request, IBinder _,
            ILogger logger)
        {
            var responseObject = new SaveEntitiesResponse {SyncedIds = new List<string>()};
            var requestData = await new StreamReader(request.Body).ReadToEndAsync();
            var requestObject = JsonConvert.DeserializeObject<List<BleScanResultDto>?>(requestData);
            
            if (requestObject == null)
            {
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                responseObject.ErrorCode = (int)HttpStatusCode.OK;
                foreach (var scanResultDto in requestObject)
                {
                    if (scanResultDto.Time.Year < 1601)
                    {
                        _errorMessageBuilder.AppendLine(Constants.DateTimeValidationErrorMessage);
                        continue;
                    }
                    if (_dataService != null && await _dataService.SaveBleScanAsync(scanResultDto))
                        responseObject.SyncedIds.Add(scanResultDto.Id);
                }
                
                if (!string.IsNullOrEmpty(_dataService?.ErrorMessage)) 
                    _errorMessageBuilder.AppendLine(_dataService.ErrorMessage);
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            if (responseObject.Message.Contains(Constants.DateTimeValidationErrorMessage)
                && responseObject.SyncedIds.Count == 0)
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
            return new JsonResult(responseObject);
        }
    }
}