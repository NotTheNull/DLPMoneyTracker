using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON
{
    internal static class AppSettings
    {
        public const string YEAR_FOLDER_PLACEHOLDER = "{YEAR}";

#if DEBUG && !TEST_NEW
        public static readonly string DATA_FOLDER_PATH = string.Format(@"C:\Users\crc\OneDrive\Programs\DLP Money Tracker\{0}\Data\", YEAR_FOLDER_PLACEHOLDER);

        public static readonly string CONFIG_FOLDER_PATH = string.Format(@"C:\Users\crc\OneDrive\Programs\DLP Money Tracker\{0}\Config\", YEAR_FOLDER_PLACEHOLDER);

        public static readonly string RECONCILE_FOLDER_PATH = string.Format(@"C:\Users\crc\OneDrive\Programs\DLP Money Tracker\{0}\Reconciliation\", YEAR_FOLDER_PLACEHOLDER);
#else

        public static string DATA_FOLDER_PATH
        {
            get
            {
                return string.Format(@"{0}\{1}\Data\", System.IO.Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER);
            }
        }

        public static string CONFIG_FOLDER_PATH
        {
            get
            {
                return string.Format(@"{0}\{1}\Config\", System.IO.Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER);
            }
        }

        public static string RECONCILE_FOLDER_PATH
        {
            get
            {
                return string.Format(@"{0}\{1}\Reconciliation\", System.IO.Directory.GetCurrentDirectory(), YEAR_FOLDER_PLACEHOLDER);
            }
        }

#endif
    }
}
