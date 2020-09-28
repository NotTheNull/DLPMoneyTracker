using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    public class AddIncomeVM : BaseViewModel
    {
        private ILedger _ledger;
        private ITrackerConfig _config;


        private DateTime _dateTrans;

        public DateTime TransactionDate
        {
            get { return _dateTrans; }
            set 
            {
                _dateTrans = value;
                NotifyPropertyChanged(nameof(this.TransactionDate));
            }
        }






        private List<SpecialDropListItem<MoneyAccount>> _listBanks = new List<SpecialDropListItem<MoneyAccount>>();
        public List<SpecialDropListItem<MoneyAccount>> BankAccountList { get { return _listBanks; } }


        public AddIncomeVM(ITrackerConfig config, ILedger ledger) : base()
        {
            _ledger = ledger;
            _config = config;
        }

    }
}
