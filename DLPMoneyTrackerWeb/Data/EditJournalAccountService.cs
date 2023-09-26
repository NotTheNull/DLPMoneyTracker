using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTrackerWeb.Data
{
    public interface IEditJournalAccountService
    {
        IEnumerable<IJournalAccount> GetMoneyAccounts();
        IEnumerable<IJournalAccount> GetLedgerAccounts();
        EditJournalAccountVM GetEditAccount(Guid idAccount);
        void SaveAccount(EditJournalAccountVM vm);
        void DeleteAccount(Guid idAccount);
    }

    public class EditJournalAccountService : IEditJournalAccountService
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

        public EditJournalAccountService(ITrackerConfig config, IJournal journal)
        {
            _config = config;
            _journal = journal;
        }


        readonly List<JournalAccountType> typesMoneyAccount = new List<JournalAccountType>() { JournalAccountType.Bank, JournalAccountType.LiabilityCard, JournalAccountType.LiabilityLoan };
        public IEnumerable<IJournalAccount> GetMoneyAccounts()
        {
            return _config.GetJournalAccountList(new JournalAccountSearch(typesMoneyAccount));
        }

        readonly List<JournalAccountType> typesLedgerAccount = new List<JournalAccountType>() { JournalAccountType.Payable, JournalAccountType.Receivable };
        public IEnumerable<IJournalAccount> GetLedgerAccounts()
        {
            return _config.GetJournalAccountList(new JournalAccountSearch(typesLedgerAccount));
        }


        public EditJournalAccountVM GetEditAccount(Guid idAccount)
        {
            if (idAccount == Guid.Empty) return new EditJournalAccountVM();

            var acct = _config.GetJournalAccount(idAccount);
            if (acct is null) throw new InvalidOperationException(string.Format("Journal Account #{0} not found", idAccount));

            EditJournalAccountVM editThis = new EditJournalAccountVM(acct);
            var initBalRecord = _journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == idAccount || x.DebitAccountId == idAccount) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            editThis.InitialBalance = initBalRecord?.TransactionAmount ?? decimal.Zero;

            return editThis;

        }

        public void SaveAccount(EditJournalAccountVM vm)
        {
            var acct = _config.GetJournalAccount(vm.Id);
            if(acct is null)
            {
                acct = JournalAccountFactory.Build(vm.Description, vm.JournalType, vm.MonthlyBudgetAmount, vm.OrderBy);
                _config.AddJournalAccount(acct);
            }
            else
            {
                JournalAccountFactory.Update(ref acct, vm.Description, vm.MonthlyBudgetAmount, vm.OrderBy);
                _config.SaveJournalAccounts();
            }


            // Create/Update Initial Balance record
            var initBalRecord = (JournalEntry?)_journal.TransactionList
                .FirstOrDefault(x =>
                    (x.CreditAccountId == acct.Id || x.DebitAccountId == acct.Id) &&
                    (x.DebitAccountId == SpecialAccount.InitialBalance.Id || x.CreditAccountId == SpecialAccount.InitialBalance.Id)
                    );
            if (initBalRecord is null)
            {
                initBalRecord = new JournalEntry(_config)
                {
                    TransactionDate = DateTime.MinValue,
                    Description = "*INITIAL BALANCE*"
                };

                if (acct.JournalType == JournalAccountType.Bank)
                {
                    initBalRecord.DebitAccount = acct;
                    initBalRecord.CreditAccount = SpecialAccount.InitialBalance;
                }
                else
                {
                    initBalRecord.DebitAccount = SpecialAccount.InitialBalance;
                    initBalRecord.CreditAccount = acct;
                }
            }
            initBalRecord.TransactionAmount = vm.InitialBalance;
            _journal.AddTransaction(initBalRecord);


        }

        public void DeleteAccount(Guid idAccount)
        {
            _config.RemoveJournalAccount(idAccount);
        }


    }

    public class EditJournalAccountVM : IJournalAccount
    {
        public EditJournalAccountVM()
        {
            this.Id = Guid.NewGuid();
            this.Description = string.Empty;
            this.JournalType = JournalAccountType.NotSet;
            this.OrderBy = 0;
            this.DateClosedUTC = null;
            this.MoneyAccountId = string.Empty;
            this.CategoryId = Guid.Empty;
            this.MonthlyBudgetAmount = decimal.Zero;
            this.InitialBalance = decimal.Zero;
        }
        public EditJournalAccountVM(IJournalAccount cpy)
        {
            this.Copy(cpy);
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get; set; }

        public int OrderBy { get; set; }

        public DateTime? DateClosedUTC { get; set; }

        public string MoneyAccountId { get; set; }

        public Guid CategoryId { get; set; }

        public decimal MonthlyBudgetAmount { get; set; }

        public decimal InitialBalance { get; set; }

        public bool ExcludeFromBudget { get; set; }

        public void Copy(IJournalAccount cpy)
        {
            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.JournalType = cpy.JournalType;
            this.OrderBy = cpy.OrderBy;
            this.DateClosedUTC = cpy.DateClosedUTC;
            this.MonthlyBudgetAmount = cpy.MonthlyBudgetAmount;
        }
    }
}
