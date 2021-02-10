using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Account
{
    // Move to Client? applicable to the whole Essentials library
    public class EssentialsDeviceInfo:IDeviceInfo
    {
        public string Manufacturer => DeviceInfo.Manufacturer;
        public string Model => DeviceInfo.Manufacturer;
        public string Platform => DeviceInfo.Platform.ToString();
        public string Version => DeviceInfo.VersionString;
    }
}
