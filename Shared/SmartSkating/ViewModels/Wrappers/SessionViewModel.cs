using Sanet.SmartSkating.Dto.Models;

namespace Sanet.SmartSkating.ViewModels.Wrappers
{
    public class SessionViewModel
    {
        private readonly SessionDto _sessionDto;

        public SessionViewModel(SessionDto sessionDto)
        {
            _sessionDto = sessionDto;
        }

        public string StartDate => _sessionDto.StartTime.ToString("yy-MM-dd h:mm");
    }
}