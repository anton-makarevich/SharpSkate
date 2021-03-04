using System.Linq;
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
                "get",
                Route = ApiNames.SessionsResource.Route)]
            HttpRequest request, ILogger logger)
        {
            var accountId = request.Query["accountId"].ToString();
            bool.TryParse(request.Query["activeOnly"].ToString(), out var activeOnly); 

            var responseObject = new GetSessionsResponse();
            
            if (string.IsNullOrEmpty(accountId))
            {
                responseObject.ErrorCode = StatusCodes.Status400BadRequest;
                _errorMessageBuilder.AppendLine(Constants.BadRequestErrorMessage);
            }
            else
            {
                var sessions = await _dataService.GetAllSessionsForAccountAsync(accountId);
                if (activeOnly)
                {
                    sessions = sessions.Where(f => !f.IsCompleted).ToList();
                }

                responseObject.Sessions = sessions;
                responseObject.ErrorCode = StatusCodes.Status200OK;
            }

            responseObject.Message = _errorMessageBuilder.ToString();
            if (responseObject.ErrorCode != 200)
                logger.LogInformation(responseObject.Message);
            return new JsonResult(responseObject);
        }
    }
}