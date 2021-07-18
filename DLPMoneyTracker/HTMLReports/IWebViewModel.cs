namespace DLPMoneyTracker.HTMLReports
{
    public interface IWebViewModel
    {
        int SelectedMonth { get; }

        string HTMLSource { get; }

        void BuildHTML();
    }
}