using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Service
{
    public static class NetworkHelper
    {
        public static bool IsConnected()
        {
            try
            {
                return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> IsConnectedAsync()
        {
            try
            {
                var current = Connectivity.Current.NetworkAccess;
                return current == NetworkAccess.Internet;
            }
            catch
            {
                return false;
            }
        }

        public static void StartMonitoring(Action<bool> onConnectivityChanged)
        {
            Connectivity.Current.ConnectivityChanged += (sender, e) =>
            {
                bool isConnected = e.NetworkAccess == NetworkAccess.Internet;
                onConnectivityChanged?.Invoke(isConnected);
            };
        }

    }
}

//usage
//// Simple check
//if (NetworkHelper.IsConnected())
//{
//    // Online operations
//}
//else
//{
//    // Offline mode
//}

//// Async check
//bool isConnected = await NetworkHelper.IsConnectedAsync();