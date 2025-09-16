using Android.Content;
using Android.OS;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using IMS_Mobile.Service;

namespace IMS_Mobile.Service
{
    public partial class SaveService
    {
        public partial void SaveAndView(string filename, string contentType, MemoryStream stream)
        {

            string exception = string.Empty;

            var context = Android.App.Application.Context;
            var cacheDir = context.CacheDir;
            Java.IO.File myDir = new(cacheDir, "IMS_Reports");
            if (!myDir.Exists())
            {
                bool created = myDir.Mkdirs();
            }
            Java.IO.File file = new(myDir, filename);

            if (file.Exists())
            {
                file.Delete();
                System.Diagnostics.Debug.WriteLine("Deleted existing file");
            }

            try
            {
                FileOutputStream outs = new(file);
                outs.Write(stream.ToArray());

                outs.Flush();
                outs.Close();
            }
            catch (Exception e)
            {
                exception = e.ToString();
            }
            if (file.Exists())
            {
                try
                {
                    var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                    if (activity != null)
                    {
                        var contentUri = AndroidX.Core.Content.FileProvider.GetUriForFile(
                            activity,
                            activity.PackageName + ".fileprovider",
                            file);

                        var shareIntent = new Intent(Intent.ActionSend);
                        shareIntent.SetType("application/pdf");
                        shareIntent.PutExtra(Intent.ExtraStream, contentUri);
                        shareIntent.PutExtra(Intent.ExtraSubject, filename);
                        shareIntent.PutExtra(Intent.ExtraText, "Transaction Report");
                        shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);

                        var chooserIntent = Intent.CreateChooser(shareIntent, "Save or Share PDF");
                        activity.StartActivity(chooserIntent);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error showing save dialog: {ex.Message}");
                }
            }

        }
    }
}
