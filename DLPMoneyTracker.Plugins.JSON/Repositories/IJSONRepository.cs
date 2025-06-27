namespace DLPMoneyTracker.Plugins.JSON.Repositories
{
    internal interface IJSONRepository
    {
        string FilePath { get; }

        void LoadFromFile();

        void SaveToFile();
    }
}