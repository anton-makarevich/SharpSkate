using System;

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

        public const string BaseUrl = "https://smartskating.azure-api.net/smartskating";
        public const string AzureApiSubscriptionKey = "<AzureApiKey>";

        public static class DevicesResource
        {
            public const string Path= "/devices";
            public const string Route = "devices";
        }

        public class AccountsResource
        {
            public const string Path= "/accounts";
            public const string Route = "accounts";
        }
    }
}
