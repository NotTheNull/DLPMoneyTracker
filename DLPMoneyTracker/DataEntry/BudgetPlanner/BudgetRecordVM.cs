using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class BudgetRecordVM : BaseViewModel, ILinkDataModelToViewModel<IBudgetRecord>
    {
        private ITrackerConfig _config;


        private Guid _id;
        public Guid UID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(nameof(this.UID));
            }
        }



        private string _desc;

        public string Description
        {
            get { return _desc; }
            set 
            { 
                _desc = value;
                NotifyPropertyChanged(nameof(this.Description));
            }
        }


        private TransactionCategory _cat;

        public TransactionCategory Category
        {
            get { return _cat; }
            set 
            { 
                _cat = value;
                NotifyPropertyChanged(nameof(this.Category));
            }
        }


        private MoneyAccount _act;

        public MoneyAccount Account
        {
            get { return _act; }
            set 
            { 
                _act = value;
                NotifyPropertyChanged(nameof(this.Account));
            }
        }




        private IScheduleRecurrence _recurr;

        public IScheduleRecurrence Recurrence
        {
            get { return _recurr; }
            set 
            { 
                _recurr = value;
                NotifyPropertyChanged(nameof(this.Recurrence));
            }
        }

        private decimal _amt;

        public decimal Amount
        {
            get { return _amt; }
            set 
            { 
                _amt = value;
                NotifyPropertyChanged(nameof(this.Amount));
            }
        }





        public BudgetRecordVM(ITrackerConfig config)
        {
            this.UID = Guid.NewGuid();
            _config = config;
        }
        public BudgetRecordVM(ITrackerConfig config, IBudgetRecord src)
        {
            this.UID = Guid.NewGuid();
            _config = config;
            this.LoadSource(src);
        }





        public IBudgetRecord GetSource()
        {
            return new BudgetRecord()
            {
                UID = this.UID,
                BillDescription = this.Description,
                Category = this.Category,
                Account = this.Account,
                Recurrence = this.Recurrence,
                ExpectedAmount = this.Amount
            };
        }

        public void LoadSource(IBudgetRecord src)
        {
            this.UID = src.UID;
            this.Description = src.BillDescription;
            this.Category = _config.GetCategory(src.CategoryID);
            this.Account = _config.GetAccount(src.AccountID);
            this.Recurrence = ScheduleRecurrenceFactory.Build(src.RecurrenceJSON);
            this.Amount = src.ExpectedAmount;
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.UID));
            NotifyPropertyChanged(nameof(this.Description));
            NotifyPropertyChanged(nameof(this.Category));
            NotifyPropertyChanged(nameof(this.Account));
            NotifyPropertyChanged(nameof(this.Amount));
            NotifyPropertyChanged(nameof(this.Recurrence));
        }
    }
}
