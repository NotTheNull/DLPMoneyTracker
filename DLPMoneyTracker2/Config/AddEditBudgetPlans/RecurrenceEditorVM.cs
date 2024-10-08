﻿using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.Core.Models.ScheduleRecurrence;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;

namespace DLPMoneyTracker2.Config.AddEditBudgetPlans
{
    public class RecurrenceEditorVM : BaseViewModel
    {
        public delegate void RecurrenceSelectedHandler(IScheduleRecurrence selected);

        public event RecurrenceSelectedHandler RecurrenceSelected;

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

        private List<SpecialDropListItem<RecurrenceFrequency>> _listFreq;

        public List<SpecialDropListItem<RecurrenceFrequency>> RecurrenceFrequencyList
        { get { return _listFreq; } }

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

        #endregion Commands

        public RecurrenceEditorVM() : base()
        {
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
            ScheduleRecurrenceFactory factory = new ScheduleRecurrenceFactory();
            return factory.Build(this.SelectedFrequency, this.StartDate);
        }

        public void EditRecurrence(IScheduleRecurrence recurr)
        {
            if (recurr is null) return;
            this.SelectedFrequency = recurr.Frequency;

            if (recurr is MonthlyRecurrence monthly)
            {
                this.StartDate = monthly.StartDate;
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