using System;
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
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class SessionProviderFunction:IAzureFunction
    {
        private readonly IDataService _dataService;
        
        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();

        public SessionProviderFunction(IDataService dataService)
        {
            _dataService = dataService;
        }

        [FunctionName("SessionProviderFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,
                "post",
                Route = ApiNames.AccountsResource.Route)]
            HttpRequest request, ILogger logger)
        {
            var responseObject = new LoginResponse();
            var requestData = await new StreamReader(request.Body).ReadToEndAsync();

            LoginRequest requestObject;
            try
            {
                requestObject = JsonConvert.DeserializeObject<LoginRequest>(requestData);
            }
            catch (Exception ex)
            {
                _errorMessageBuilder.AppendLine(ex.Message);
                requestObject = default;
            }

            if (requestObject.Equals(default(LoginRequest)))
            {
                responseObject.ErrorCode = (int) HttpStatusCode.BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                await _dataService.GetAllSessionsForAccountAsync("");
                responseObject.ErrorCode = responseObject.Account == null
                    ? (int) HttpStatusCode.NotFound
                    : (int) HttpStatusCode.OK;
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            if (responseObject.ErrorCode != 200)
                logger.LogInformation(responseObject.Message);
            return new JsonResult(responseObject);
        }
    }
}