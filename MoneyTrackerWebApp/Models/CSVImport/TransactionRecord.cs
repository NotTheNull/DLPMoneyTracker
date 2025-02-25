using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace MoneyTrackerWebApp.Models.CSVImport
{
    public class TransactionRecord
    {
        private readonly Guid moneyAccountUID;
        private IMoneyTransaction trans;

        public TransactionRecord(IMoneyTransaction trans, Guid moneyAccountUID)
        {
            this.trans = trans;
            this.moneyAccountUID = moneyAccountUID;
        }

        public Guid UID { get; set; } = Guid.NewGuid();
        public bool IsSelected { get; set; }
        public IMoneyTransaction MoneyRecord { get { return trans; } }
        public Guid TransactionId { get { return trans.UID; } }

        public DateTime TransactionDate { get { return trans.TransactionDate; } }


        protected SingleAccountTransaction Record { get { return trans.Records.Where(x => x.Account.Id != moneyAccountUID).FirstOrDefault(); } }
        protected IJournalAccount Account { get { return Record.Account; } }
        public string AccountName { get { return Account.Description; } }

        public string Description { get { return trans.Description; } }

        public decimal Amount { get { return trans.TransactionAmount; } }

        public DateTime? BankDate
        {
            get { return Record.BankDate; }
            set
            {
                Record.BankDate = value;
            }
        }

        public void IsSelectedChanged(bool value)
        {
            this.IsSelected = value;
        }

        public void BankDateChanged(DateTime? newDate)
        {
            this.BankDate = newDate;
        }


    }
}
