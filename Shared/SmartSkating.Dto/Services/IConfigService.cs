namespace Sanet.SmartSkating.Dto.Services
{
    public interface IConfigService
    {
        public string BaseUrl { get; }
        public string AzureApiSubscriptionKey { get; }
    }
}