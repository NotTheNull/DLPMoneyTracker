#define TEST_NEW


namespace DLPMoneyTracker.Data
{
    /// <summary>
    /// These are the settings that are, at some point, expected to be stored in a app related config file
    /// </summary>
    public static class AppConfigSettings
    {
        public const string YEAR_FOLDER_PLACEHOLDER = "{YEAR}";

#if DEBUG && !TEST_NEW
        public static readonly string DATA_FOLDER_PATH = string.Format(@"C:\Users\Landon\OneDrive\Programs\DLP Money Tracker\{0}\Data\", YEAR_FOLDER_PLACEHOLDER);

        public static readonly string CONFIG_FOLDER_PATH = string.Format(@"C:\Users\Landon\OneDrive\Programs\DLP Money Tracker\{0}\Config\", YEAR_FOLDER_PLACEHOLDER);
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

#endif
    }
}