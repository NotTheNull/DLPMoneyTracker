using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.BankReconciliation;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DLPMoneyTracker2.Main.BankReconciliation
{
    public class BankFileVM : BaseViewModel
    {

        #region Properties

        private IJournalAccount account = null!;

        public Guid AccountId => account.Id;
        public string AccountDescription => account.Description;

        private decimal? _initBal = null;

        public decimal? InitialBalance
        {
            get { return _initBal; }
            set
            {
                _initBal = value;
                NotifyPropertyChanged(nameof(InitialBalance));
            }
        }

        private decimal? _janBal = null;

        public decimal? JanuaryBalance
        {
            get { return _janBal; }
            set
            {
                _janBal = value;
                NotifyPropertyChanged(nameof(JanuaryBalance));
            }
        }

        private decimal? _febBal = null;

        public decimal? FebruaryBalance
        {
            get { return _febBal; }
            set
            {
                _febBal = value;
                NotifyPropertyChanged(nameof(FebruaryBalance));
            }
        }

        private decimal? _marBal = null;

        public decimal? MarchBalance
        {
            get { return _marBal; }
            set
            {
                _marBal = value;
                NotifyPropertyChanged(nameof(MarchBalance));
            }
        }

        private decimal? _aprBal = null;

        public decimal? AprilBalance
        {
            get { return _aprBal; }
            set
            {
                _aprBal = value;
                NotifyPropertyChanged(nameof(AprilBalance));
            }
        }

        private decimal? _mayBal = null;

        public decimal? MayBalance
        {
            get { return _mayBal; }
            set
            {
                _mayBal = value;
                NotifyPropertyChanged(nameof(MayBalance));
            }
        }

        private decimal? _juneBal = null;

        public decimal? JuneBalance
        {
            get { return _juneBal; }
            set
            {
                _juneBal = value;
                NotifyPropertyChanged(nameof(JuneBalance));
            }
        }

        private decimal? _julyBal = null;

        public decimal? JulyBalance
        {
            get { return _julyBal; }
            set
            {
                _julyBal = value;
                NotifyPropertyChanged(nameof(JulyBalance));
            }
        }

        private decimal? _augBal = null;

        public decimal? AugustBalance
        {
            get { return _augBal; }
            set
            {
                _augBal = value;
                NotifyPropertyChanged(nameof(AugustBalance));
            }
        }

        private decimal? _septBal = null;

        public decimal? SeptemberBalance
        {
            get { return _septBal; }
            set
            {
                _septBal = value;
                NotifyPropertyChanged(nameof(SeptemberBalance));
            }
        }

        private decimal? _octBal = null;

        public decimal? OctoberBalance
        {
            get { return _octBal; }
            set
            {
                _octBal = value;
                NotifyPropertyChanged(nameof(OctoberBalance));
            }
        }

        private decimal? _novBal = null;

        public decimal? NovemberBalance
        {
            get { return _novBal; }
            set
            {
                _novBal = value;
                NotifyPropertyChanged(nameof(NovemberBalance));
            }
        }

        private decimal? _decBal = null;

        public decimal? DecemberBalance
        {
            get { return _decBal; }
            set
            {
                _decBal = value;
                NotifyPropertyChanged(nameof(DecemberBalance));
            }
        }

        #endregion

        #region Commands

        public RelayCommand CommandStartNewReconciliation =>
            new((o) =>
            {
                var windowBankRec = UICore.DependencyHost.GetRequiredService<BankReconciliationUI>();
                windowBankRec.LoadAccount(account);
                windowBankRec.Show();
            });

        #endregion

        public void LoadBankReconciliation(BankReconciliationOverviewDTO dto)
        {
            account = dto.BankAccount;
            NotifyPropertyChanged(nameof(this.AccountId));
            NotifyPropertyChanged(nameof(this.AccountDescription));

            if (dto.ReconciliationList?.Any() != true) return;

            var firstRecord = dto.ReconciliationList.OrderBy(o => o.StatementDate.End).FirstOrDefault();
            this.InitialBalance = firstRecord?.StartingBalance ?? decimal.Zero;
            this.JanuaryBalance = dto.GetRecordForMonth(1)?.EndingBalance;
            this.FebruaryBalance = dto.GetRecordForMonth(2)?.EndingBalance;
            this.MarchBalance = dto.GetRecordForMonth(3)?.EndingBalance;
            this.AprilBalance = dto.GetRecordForMonth(4)?.EndingBalance;
            this.MayBalance = dto.GetRecordForMonth(5)?.EndingBalance;
            this.JuneBalance = dto.GetRecordForMonth(6)?.EndingBalance;
            this.JulyBalance = dto.GetRecordForMonth(7)?.EndingBalance;
            this.AugustBalance = dto.GetRecordForMonth(8)?.EndingBalance;
            this.SeptemberBalance = dto.GetRecordForMonth(9)?.EndingBalance;
            this.OctoberBalance = dto.GetRecordForMonth(10)?.EndingBalance;
            this.NovemberBalance = dto.GetRecordForMonth(11)?.EndingBalance;
            this.DecemberBalance = dto.GetRecordForMonth(12)?.EndingBalance;
        }
    }
}