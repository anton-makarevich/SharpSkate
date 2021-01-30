namespace Sanet.SmartSkating.Services
{
    public class SettingsService:ISettingsService
    {
        public bool UseGps { get; set; } = true;
        public bool UseBle { get; set; }
        
        public bool CanInterpolateSectors { get; set; }
    }
}