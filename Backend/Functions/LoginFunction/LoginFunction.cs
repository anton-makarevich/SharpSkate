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
using Sanet.SmartSkating.Dto.Models.Requests;
using Sanet.SmartSkating.Dto.Models.Responses;
using Sanet.SmartSkating.Dto.Services.Account;

namespace Sanet.SmartSkating.Backend.Functions
{
    public class LoginFunction:IAzureFunction
    {
        private ILoginService? _loginService;
        
        private readonly StringBuilder _errorMessageBuilder = new StringBuilder();

        public void SetService(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, 
            "post",
            Route = ApiNames.AccountsResource.Route)]HttpRequest request, ILogger log)
        {
            if (_loginService == null)
                SetService(new AzureLoginService());

            var responseObject = new LoginResponse();
            var requestData = await new StreamReader(request.Body).ReadToEndAsync();
            
            var requestObject = JsonConvert.DeserializeObject<LoginRequest>(requestData);
            
            if (requestObject.Equals(default))
            {
                responseObject.ErrorCode = (int)HttpStatusCode.BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                responseObject.ErrorCode = (int)HttpStatusCode.OK;
                
                if (_loginService != null)
                    responseObject.Account = await _loginService.LoginUserAsync(
                        requestObject.Username,
                        requestObject.Password);
                
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            return new JsonResult(responseObject);
        }
    }
}