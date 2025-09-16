using System;
using System.IO;
using IMS_Mobile.Service;

namespace IMS_Mobile.Service
{
    public partial class SaveService
    {
        public partial void SaveAndView(string filename, string contentType, MemoryStream stream)
        {
            // iOS implementation - not used since app is Android-only
            System.Diagnostics.Debug.WriteLine("iOS SaveAndView called - not implemented for Android-only app");
        }
    }
}
