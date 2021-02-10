using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Account
{
    public class EssentialsPreferences:IPreferences
    {
        public string Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set(string key, string value)
        {
            Preferences.Set(key,value);
        }
    }
}
