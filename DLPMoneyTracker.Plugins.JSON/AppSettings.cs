
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DLPMoneyTracker.Plugins.JSON
{
    internal static class AppSettings
    {
        public const string YEAR_FOLDER_PLACEHOLDER = "{YEAR}";

#if DEBUG && !TEST_NEW
        private static readonly string ONEDRIVE_PATH = @"C:\Users\crc\OneDrive\Programs\DLP Money Tracker\";

        public static readonly string OLD_DATA_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, YEAR_FOLDER_PLACEHOLDER, "Data");
        public static readonly string OLD_CONFIG_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, YEAR_FOLDER_PLACEHOLDER, "Config");
        public static readonly string OLD_RECONCILE_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, YEAR_FOLDER_PLACEHOLDER, "Reconciliation");
        public static readonly string NEW_DATA_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, "Data");
        public static readonly string NEW_CONFIG_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, "Config");
        public static readonly string NEW_RECONCILE_FOLDER_PATH = Path.Combine(ONEDRIVE_PATH, "Reconciliation");
#else

        public static readonly string OLD_DATA_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER, "Data");
        public static readonly string OLD_CONFIG_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER, "Config");
        public static readonly string OLD_RECONCILE_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER, "Reconciliation");
        public static readonly string NEW_DATA_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        public static readonly string NEW_CONFIG_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        public static readonly string NEW_RECONCILE_FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Reconciliation");


#endif
    }
}
