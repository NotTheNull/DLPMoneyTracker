
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.Collections.Generic;

namespace DLPMoneyTracker2.LedgerEntry
{
    public class ExpenseJournalEntryVM : BaseRecordJournalEntryVM
    {
        public ExpenseJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase) :
            base(
                getAccountsByTypeUseCase,
                getAccountByUIDUseCase,
                saveMoneyRecordUseCase,
                new List<LedgerType>() { LedgerType.Payable }, 
                new List<LedgerType>() { LedgerType.Bank, LedgerType.LiabilityCard },
                TransactionType.Expense)
        {
        }

        public override string Title => "Enter Expense";
        public override bool IsValidTransaction
        {
            get
            {
                return this.SelectedDebitAccount.JournalType == LedgerType.Payable
                    && !string.IsNullOrWhiteSpace(this.Description)
                    && this.Amount > decimal.Zero;
            }
        }

        public override string CreditHeader
        { get { return "Money Account"; } }

        public override string DebitHeader
        { get { return "Payable"; } }
    }
}