namespace DLPMoneyTracker.Data
{
    /// <summary>
    /// These are the settings that are, at some point, expected to be stored in a app related config file
    /// </summary>
    public static class AppConfigSettings
    {
        public const string YEAR_FOLDER_PLACEHOLDER = "{YEAR}";
        // TODO: Modify program to store the Data Folder Path in a config file
        public static readonly string DATA_FOLDER_PATH = string.Format(@"D:\Program Files\DLP Money Tracker\{0}\Data\", YEAR_FOLDER_PLACEHOLDER);

        // TODO: Modify program to store the Tracker Config Path in a config file
        public static readonly string CONFIG_FOLDER_PATH = string.Format(@"D:\Program Files\DLP Money Tracker\{0}\Config\", YEAR_FOLDER_PLACEHOLDER);
    }
}