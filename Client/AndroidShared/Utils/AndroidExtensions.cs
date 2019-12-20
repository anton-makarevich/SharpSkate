using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;

namespace Sanet.SmartSkating.Droid.Utils
{
    public static class AndroidExtensions
    {
        public static int PermissionsRequestCode = 432;
        public static Permission[] RequestPermissions(this Activity activity)
        {
            var permissions = new List<string>();

            var locationPermission = ContextCompat.CheckSelfPermission(activity,
                Manifest.Permission.AccessFineLocation);
            if (locationPermission != Permission.Granted)
            {
                permissions.Add(Manifest.Permission.AccessFineLocation);
            }

            if (permissions.Any())
            {
                ActivityCompat.RequestPermissions(activity,
                    permissions.ToArray(), PermissionsRequestCode);
            }

            return new[] {locationPermission};
        }
    }
}