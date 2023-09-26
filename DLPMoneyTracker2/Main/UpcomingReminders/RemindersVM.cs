using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.UpcomingReminders
{
    public class RemindersVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;
        private readonly IJournalPlanner _planner;

        public RemindersVM(ITrackerConfig config, IJournal journal, IJournalPlanner planner)
        {
            _config = config;
            _journal = journal;
            _planner = planner;

            _journal.JournalModified += _journal_JournalModified;
            _listBills = new ObservableCollection<BillDetailVM>();
        }

        private void _journal_JournalModified()
        {
            this.Load();
        }

        //public ObservableCollection<BillDetailVM> RemindersList { get; set; }
        private ObservableCollection<BillDetailVM> _listBills;

        public ObservableCollection<BillDetailVM> RemindersList
        {
            get { return _listBills; }
        }

        public void Load()
        {
            this.RemindersList.Clear();

            DateRange range = new DateRange(DateTime.Today.Year, DateTime.Today.Month);
            var listPlans = _planner.GetPlansForDateRange(range);
            if (listPlans?.Any() != true) return;

            foreach (var plan in listPlans.OrderBy(o => o.NextOccurrence))
            {
                // See if we already have a transaction for this plan
                var account = _config.GetJournalAccount(plan.CreditAccountId);
                var transactions = _journal.Search(new JournalSearchFilter(plan, account));
                if (transactions?.Any() == true) continue;

                this.RemindersList.Add(new BillDetailVM(plan));
            }
        }
    }
}