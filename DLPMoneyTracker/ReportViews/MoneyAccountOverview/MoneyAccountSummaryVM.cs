using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.ReportViews
{
    public class MoneyAccountSummaryVM : BaseViewModel
    {
        private ILedger _ledger;
        private MoneyAccount _act;

        public string AccountID { get { return _act?.ID ?? string.Empty; } }
        public string AccountDesc { get { return _act?.Description ?? string.Empty; } }

        public MoneyAccountType AccountType { get { return _act?.AccountType ?? MoneyAccountType.NotSet; } }

        private decimal _bal;

        public decimal Balance
        {
            get { return _bal; }
            set
            {
                _bal = value;
                NotifyPropertyChanged(nameof(this.Balance));
            }
        }




        #region Commands
        private RelayCommand _cmdDetails;
        public RelayCommand CommandDetails
        {
            get
            {
                return _cmdDetails ?? (_cmdDetails = new RelayCommand((o) =>
                {
                    // TODO: Command Details needs to display the ledger transactions associated with the given account

                }));
            }
        }


        #endregion




        public MoneyAccountSummaryVM(MoneyAccount act, ILedger ledger)
        {
            _ledger = ledger;
            _ledger.LedgerModified += () => Refresh();
            _act = act;

            this.Refresh();
        }


        public void Refresh()
        {
            this.Balance = _ledger.GetAccountBalance(_act);
            this.NotifyAll();
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.AccountID));
            NotifyPropertyChanged(nameof(this.AccountDesc));
            NotifyPropertyChanged(nameof(this.AccountType));
            NotifyPropertyChanged(nameof(this.Balance));
        }
    }
}
