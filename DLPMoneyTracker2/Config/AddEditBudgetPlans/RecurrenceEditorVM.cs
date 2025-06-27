using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.Config.AddEditBudgetPlans
{
    public class RecurrenceEditorVM : BaseViewModel
    {
        public delegate void RecurrenceSelectedHandler(IScheduleRecurrence selected);
        public event RecurrenceSelectedHandler? RecurrenceSelected;

        private RecurrenceFrequency _selFreq;
        public RecurrenceFrequency SelectedFrequency
        {
            get { return _selFreq; }
            set
            {
                _selFreq = value;
                NotifyPropertyChanged(nameof(this.SelectedFrequency));
                NotifyPropertyChanged(nameof(this.IsMonthly));
            }
        }

        public bool IsMonthly => this.SelectedFrequency == RecurrenceFrequency.Monthly;


        private DateTime _dateStart = DateTime.Today;
        public DateTime StartDate
        {
            get { return _dateStart; }
            set
            {
                _dateStart = value;
                NotifyPropertyChanged(nameof(this.StartDate));
            }
        }

        private readonly List<SpecialDropListItem<RecurrenceFrequency>> _listFreq = 
        [
            new SpecialDropListItem<RecurrenceFrequency>("Monthly", RecurrenceFrequency.Monthly),
            new SpecialDropListItem<RecurrenceFrequency>("Semi-Annual", RecurrenceFrequency.SemiAnnual),
            new SpecialDropListItem<RecurrenceFrequency>("Annual", RecurrenceFrequency.Annual)
        ];
        public List<SpecialDropListItem<RecurrenceFrequency>> RecurrenceFrequencyList => _listFreq;

        #region Commands

        public RelayCommand CommandSave => 
            new((o) =>
            {
                RecurrenceSelected?.Invoke(this.GetRecurrence());
            });

        #endregion Commands

        public RecurrenceEditorVM() : base() { }

        public IScheduleRecurrence GetRecurrence()
        {
            return ScheduleRecurrenceFactory.Build(this.SelectedFrequency, this.StartDate);
        }

        public void EditRecurrence(IScheduleRecurrence r)
        {
            if (r is null) return;
            this.SelectedFrequency = r.Frequency;

            if (r is MonthlyRecurrence monthly)
            {
                this.StartDate = monthly.StartDate;
            }
            else if (r is SemiAnnualRecurrence semi)
            {
                this.StartDate = semi.StartDate;
            }
            else if (r is AnnualRecurrence annual)
            {
                this.StartDate = annual.StartDate;
            }
        }
    }
}