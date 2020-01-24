namespace Sanet.SmartSkating.Dto
{
    public static class ApiNames
    {
        public static class WayPointsResource
        {
            public const string Path= "/waypoints";
            public const string Route = "waypoints";
        }
        
        public static class SessionsResource
        {
            public const string Path= "/sessions";
            public const string Route = "sessions";
        }
        
        public static class BleScansResource
        {
            public const string Path= "/scans";
            public const string Route = "scans";
        }

        public static readonly string BaseUrl = "https://smartskating.azure-api.net/smartskating";
        public static readonly string AzureApiSubscriptionKey = "<Ocp-Apim-Subscription-Key>";
    }
}