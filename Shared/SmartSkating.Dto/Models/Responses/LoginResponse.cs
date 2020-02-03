using Sanet.SmartSkating.Dto.Models.Responses.Base;

namespace Sanet.SmartSkating.Dto.Models.Responses
{
    public class LoginResponse:ResponseBase
    {
        public AccountDto? Account { get; set; }
    }
}