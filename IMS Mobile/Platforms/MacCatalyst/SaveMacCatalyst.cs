using System;
using System.IO;
using IMS_Mobile.Service;

namespace IMS_Mobile.Service
{
    public partial class SaveService
    {
        public partial void SaveAndView(string filename, string contentType, MemoryStream stream)
        {
            // MacCatalyst implementation - not used since app is Android-only
            System.Diagnostics.Debug.WriteLine("MacCatalyst SaveAndView called - not implemented for Android-only app");
        }
    }
}
