using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.DB
{
    public static class Constants
    {
        private const string DBFileName = "IMS_Mobile.db3";

        public const SQLiteOpenFlags Flags =
             SQLiteOpenFlags.ReadWrite |
             SQLiteOpenFlags.Create |
             SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var appDataDir = FileSystem.AppDataDirectory;

                // Ensure the directory exists
                if (!Directory.Exists(appDataDir))
                {
                    Directory.CreateDirectory(appDataDir);
                }

                var dbPath = Path.Combine(appDataDir, DBFileName);

                if (!File.Exists(dbPath))
                {
                    try
                    {
                        File.WriteAllText(dbPath, "");
                        Debug.WriteLine($"[DB] Created database file at: {dbPath}");
                    }
                    catch (Exception ex)
                    {
                       Debug.WriteLine($"[ERROR][DB] Failed to create database file: {ex.Message}");
                    }
                }

                return dbPath;
            }
        }
    }
}
