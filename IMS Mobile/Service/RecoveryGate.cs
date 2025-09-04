using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Maui.Storage;

namespace IMS_Mobile.Service
{
    public static class RecoveryGate
    {
        const string Key = "recovery_gate_until";
        public static void Begin(int seconds = 90)
            => Preferences.Set(Key, DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds());
        public static bool IsActive()
        {
            var until = Preferences.Get(Key, 0L);
            return until > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public static void End() => Preferences.Remove(Key);
    }
}
