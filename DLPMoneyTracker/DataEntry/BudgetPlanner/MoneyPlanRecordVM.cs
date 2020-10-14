using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class MoneyPlanRecordVM : BaseViewModel, ILinkDataModelToViewModel<IMoneyPlan>
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
                NotifyPropertyChanged(nameof(this.CategoryName));
            }
        }

        public string CategoryName { get { return this.Category?.Name ?? string.Empty; } }
        public CategoryType CategoryType { get { return this.Category?.CategoryType ?? CategoryType.NotSet; } }

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
                NotifyPropertyChanged(nameof(this.NextDueDate));
                NotifyPropertyChanged(nameof(this.NotificationDate));
            }
        }

        public DateTime NextDueDate { get { return this.Recurrence?.NextOccurence ?? DateTime.MinValue; } }
        public DateTime NotificationDate { get { return this.Recurrence?.NotificationDate ?? DateTime.MinValue; } }

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

        public MoneyPlanRecordVM(ITrackerConfig config)
        {
            this.UID = Guid.NewGuid();
            _config = config;
        }

        public MoneyPlanRecordVM(ITrackerConfig config, IMoneyPlan src)
        {
            this.UID = Guid.NewGuid();
            _config = config;
            this.LoadSource(src);
        }

        public IMoneyPlan GetSource()
        {
            return MoneyPlanFactory.Build(this.Category, this.Account, this.Description, this.Recurrence, this.Amount, this.UID);
        }

        public void LoadSource(IMoneyPlan src)
        {
            this.UID = src.UID;
            this.Description = src.Description;
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