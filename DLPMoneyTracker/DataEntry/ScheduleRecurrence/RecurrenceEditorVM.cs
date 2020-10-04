using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.ScheduleRecurrence
{
    public class RecurrenceEditorVM : BaseViewModel
    {
        public delegate void RecurrenceSelectedHandler(IScheduleRecurrence selected);
        public event RecurrenceSelectedHandler RecurrenceSelected;
        public event SimpleNotification EditCanceled;




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


        public bool IsMonthly
        {
            get { return this.SelectedFrequency == RecurrenceFrequency.Monthly; }
        }


        private int _monthDay;

        public int DayOfMonth
        {
            get { return _monthDay; }
            set
            {
                if (value < 1) _monthDay = 1;
                else if (value > 31) _monthDay = 31;
                else _monthDay = value;
                NotifyPropertyChanged(nameof(this.DayOfMonth));
            }
        }


        private DateTime _dateStart;

        public DateTime StartDate
        {
            get { return _dateStart; }
            set
            {
                _dateStart = value;
                NotifyPropertyChanged(nameof(this.StartDate));
            }
        }




        List<SpecialDropListItem<RecurrenceFrequency>> _listFreq;
        public List<SpecialDropListItem<RecurrenceFrequency>> RecurrenceFrequencyList { get { return _listFreq; } }



        #region Commands
        private RelayCommand _cmdSave;
        public RelayCommand CommandSave
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
              {
                  RecurrenceSelected?.Invoke(this.GetRecurrence());
              }));
            }
        }

        private RelayCommand _cmdClose;
        public RelayCommand CommandClose
        {
            get
            {
                return _cmdClose ?? (_cmdClose = new RelayCommand((o) =>
                {
                    EditCanceled?.Invoke();
                }));
            }
        }
        #endregion


        public RecurrenceEditorVM() : base()
        {
            _monthDay = 1;
            _dateStart = DateTime.Today;
            this.LoadFrequenceList();
        }


        private void LoadFrequenceList()
        {
            _listFreq = new List<SpecialDropListItem<RecurrenceFrequency>>()
            {
                new SpecialDropListItem<RecurrenceFrequency>("Monthly", RecurrenceFrequency.Monthly),
                new SpecialDropListItem<RecurrenceFrequency>("Semi-Annual", RecurrenceFrequency.SemiAnnual),
                new SpecialDropListItem<RecurrenceFrequency>("Annual", RecurrenceFrequency.Annual)
            };
        }

        public IScheduleRecurrence GetRecurrence()
        {
            if (this.IsMonthly) return ScheduleRecurrenceFactory.Build(this.SelectedFrequency, this.DayOfMonth);

            return ScheduleRecurrenceFactory.Build(this.SelectedFrequency, this.StartDate);
        }

        public void EditRecurrence(IScheduleRecurrence recurr)
        {
            this.SelectedFrequency = recurr.Frequency;

            if (recurr is MonthlyRecurrence monthly)
            {
                this.DayOfMonth = monthly.DayOfMonth;
            }
            else if (recurr is SemiAnnualRecurrence semi)
            {
                this.StartDate = semi.StartDate;
            }
            else if (recurr is AnnualRecurrence annual)
            {
                this.StartDate = annual.StartDate;
            }
        }


    }
}
