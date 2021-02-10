namespace Sanet.SmartSkating.Services.Account
{
    public interface IPreferences
    {
        string Get(string key, string defaultValue);
        void Set(string key, string value);
    }
}
