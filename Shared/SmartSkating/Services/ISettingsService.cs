namespace Sanet.SmartSkating.Services
{
    public interface ISettingsService
    {
        bool UseGps { get; set; }
        bool UseBle { get; set; }

        bool CanInterpolateSectors { get; set; }
    }
}