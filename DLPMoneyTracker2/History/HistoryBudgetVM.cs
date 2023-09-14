using DLPMoneyTracker.Data;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.History
{
    
    // TODO: Finish History UI
    public class HistoryBudgetVM : BaseViewModel
    {
        // NOTE: These cannot be from the IOC as we may need to change them to pull a different Year
        private ITrackerConfig _config;
        private IJournal _journal;

        public HistoryBudgetVM()
        {
            _listMonths = new List<SpecialDropListItem<int>>()
            {
                new SpecialDropListItem<int>("January", 1),
                new SpecialDropListItem<int>("February", 2),
                new SpecialDropListItem<int>("March", 3),
                new SpecialDropListItem<int>("April", 4),
                new SpecialDropListItem<int>("May", 5),
                new SpecialDropListItem<int>("June", 6),
                new SpecialDropListItem<int>("July", 7),
                new SpecialDropListItem<int>("August", 8),
                new SpecialDropListItem<int>("September", 9),
                new SpecialDropListItem<int>("October", 10),
                new SpecialDropListItem<int>("November", 11),
                new SpecialDropListItem<int>("December", 12)
            };
        }


        #region Editing Controls

        private int _year;

        public int SelectedYear
        {
            get { return _year; }
            set 
            {
                _year = value;
                NotifyPropertyChanged(nameof(SelectedYear));
            }
        }

        public List<SpecialDropListItem<int>> _listMonths;
        public ReadOnlyCollection<SpecialDropListItem<int>> MonthList
        {
            get { return _listMonths.AsReadOnly(); }
        }

        private int _month;

        public int SelectedMonth
        {
            get { return _month; }
            set 
            { 
                _month = value;
                NotifyPropertyChanged(nameof(SelectedMonth));
            }
        }

        #endregion






        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(SelectedYear));
            NotifyPropertyChanged(nameof(SelectedMonth));
        }
    }
}
