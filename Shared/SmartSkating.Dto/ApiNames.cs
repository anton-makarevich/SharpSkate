using System.Net.Http;

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

        public static class DevicesResource
        {
            public const string Path= "/devices";
            public const string Route = "devices";
        }

        public static class AccountsResource
        {
            public const string Path= "/accounts";
            public const string Route = "accounts";
        }
    }
}
