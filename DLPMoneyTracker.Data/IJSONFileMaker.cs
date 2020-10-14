namespace DLPMoneyTracker.Data
{
    public interface IJSONFileMaker
    {
        string FilePath { get; }

        void LoadFromFile();

        void SaveToFile();
    }
}