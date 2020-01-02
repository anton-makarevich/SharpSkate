using System;
using Xamarin.Essentials;

namespace Sanet.SmartSkating.Services.Account
{
    public class EssentialsAccountService:IAccountService
    {
        private const string userIdKey = "userId";
        public string UserId
        {
            get
            {
                var userId = Preferences.Get(userIdKey,string.Empty);

                if (!string.IsNullOrEmpty(userId)) return userId;
                
                userId = Guid.NewGuid().ToString("N");
                Preferences.Set(userIdKey,userId);

                return userId;
            }
        }
    }
}