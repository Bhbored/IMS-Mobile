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
                return Path
                     .Combine(FileSystem.AppDataDirectory, DBFileName);
            }
        }
    }
}
