﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            RemindersList = new ObservableCollection<BillDetailVM>();
        }


        public ObservableCollection<BillDetailVM> RemindersList { get; set; }


        public void Load()
        {
            this.RemindersList.Clear();   
            
            DateRange range = new DateRange(DateTime.Today.Year, DateTime.Today.Month);
            var listPlans = _planner.GetPlansForDateRange(range);
            if (listPlans?.Any() != true) return;

            foreach(var plan in listPlans)
            {
                // See if we already have a transaction for this plan
                var account = _config.LedgerAccountsList.FirstOrDefault(x => x.Id == plan.CreditAccountId);
                var transactions = _journal.Search(new JournalSearchFilter(plan, account));
                if (transactions?.Any() == true) continue;

                this.RemindersList.Add(new BillDetailVM(plan));
            }

        }
    }
}