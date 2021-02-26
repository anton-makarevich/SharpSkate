using System;

namespace Sanet.SmartSkating.Dto.Services
{
    public class DateProvider:IDateProvider
    {
        public DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}