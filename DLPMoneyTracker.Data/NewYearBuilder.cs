using System;

namespace DLPMoneyTracker.Data
{
    public class NewYearBuilder
    {
        /// <summary>
        /// Copies config and Ledger accounts to the new year.  Creates new journal entries with the current balances.
        /// This feature can be run at any time and will simply rebuild next year's data.
        /// </summary>
        /// <param name="newYear"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void SetupNewYear()
        {
            NewYearBuilder buildIt = new NewYearBuilder(DateTime.Today.Year + 1);
            buildIt.BuildNewConfig();
            buildIt.BuildNewPlanner();
            buildIt.BuildNewLedger();
        }

        public static void RebuildCurrentYear()
        {
            NewYearBuilder buildIt = new NewYearBuilder(DateTime.Today.Year);
            buildIt.BuildNewConfig();
            buildIt.BuildNewPlanner();
            buildIt.BuildNewLedger();
        }

        private ITrackerConfig _oldConfig, _newConfig;
        private int _newYear;

        private NewYearBuilder(int newYear)
        {
            _newYear = newYear;
        }

        protected void BuildNewConfig()
        {
            _oldConfig = new TrackerConfig(_newYear - 1);
            _newConfig = new TrackerConfig(_newYear);

            _newConfig.Copy(_oldConfig);
            _newConfig.SaveJournalAccounts();
        }

        protected void BuildNewPlanner()
        {
            JournalPlanner oldPlanner = new JournalPlanner(_oldConfig, _newYear - 1);
            JournalPlanner newPlanner = new JournalPlanner(_newConfig, _newYear);
            newPlanner.Copy(oldPlanner);
            newPlanner.SaveToFile();
        }

        protected void BuildNewLedger()
        {
            DLPJournal oldJournal = new DLPJournal(_oldConfig, _newYear - 1);
            DLPJournal newJournal = new DLPJournal(_newConfig, _newYear);

            newJournal.BuildInitialBalances(oldJournal);
            newJournal.SaveToFile();
        }
    }
}