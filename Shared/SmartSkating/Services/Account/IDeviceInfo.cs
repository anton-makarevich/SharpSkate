namespace Sanet.SmartSkating.Services.Account
{
    public interface IDeviceInfo
    {
        string Manufacturer { get; }
        string Model { get; }
        string Platform { get; }
        string Version { get; }
    }
}
