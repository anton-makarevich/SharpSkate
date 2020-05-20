using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;

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

            if (permissions.Count>0)
            {
                ActivityCompat.RequestPermissions(activity,
                    permissions.ToArray(), PermissionsRequestCode);
            }

            return new[] {locationPermission};
        }
    }
}