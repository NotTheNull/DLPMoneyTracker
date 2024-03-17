using DLPMoneyTracker.Data;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.YTD
{
    public class YearToDateVM : BaseViewModel
    {
        // Plan here is to break down each Payables and Receivables by month with a YTD total in a data grid
        // Will need to have a Drill Down option to get the list of transactions for a given month

        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;
        private readonly int _year; // Exists for if we decide to use this UI for History

        public YearToDateVM(ITrackerConfig config, IJournal journal)
        {
            _config = config;
            _journal = journal;
            _year = DateTime.Today.Year;

            IncomeAccountDetailList = new ObservableCollection<YTDAccountDetailVM>();
            ExpenseAccountDetailList = new ObservableCollection<YTDAccountDetailVM>();
            _journal.JournalModified += _journal_JournalModified;

            this.Load();
        }

        private void _journal_JournalModified()
        {
            this.Load();
        }

        public ObservableCollection<YTDAccountDetailVM> IncomeAccountDetailList { get; set; }
        public ObservableCollection<YTDAccountDetailVM> ExpenseAccountDetailList { get; set; }

        private void Load()
        {
            IncomeAccountDetailList.Clear();
            ExpenseAccountDetailList.Clear();

            JournalAccountSearch search = new JournalAccountSearch();
            search.IncludeDeleted = true;
            search.JournalTypes.Add(DLPMoneyTracker.Data.LedgerAccounts.LedgerType.Receivable);
            search.JournalTypes.Add(DLPMoneyTracker.Data.LedgerAccounts.LedgerType.Payable);

            var listAccounts = _config.GetJournalAccountList(search);
            if (listAccounts?.Any() != true) return;

            foreach (var act in listAccounts)
            {
                if (act.JournalType == DLPMoneyTracker.Data.LedgerAccounts.LedgerType.Receivable)
                {
                    IncomeAccountDetailList.Add(new YTDAccountDetailVM(_year, act, _config, _journal));
                }
                else
                {
                    ExpenseAccountDetailList.Add(new YTDAccountDetailVM(_year, act, _config, _journal));
                }
            }
        }
    }
}