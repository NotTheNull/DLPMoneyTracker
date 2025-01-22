using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System.ComponentModel.DataAnnotations;

namespace MoneyTrackerWebApp.Models.Transactions
{
    public class EditTransactionVM : IMoneyTransaction
    {
        public Guid UID { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        public TransactionType JournalEntryType { get; set; }

        [Required]
        public IJournalAccount DebitAccount { get; set; }

        private Guid _debit;
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? _debit; } set { _debit = value; } }

        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }

        public DateTime? DebitBankDate { get; set; }

        [Required]
        public IJournalAccount CreditAccount { get; set; }

        private Guid _credit;
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? _credit; } set { _credit = value; } }

        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }

        public DateTime? CreditBankDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01d, 99999.99d, ErrorMessage = "Please enter a reasonable dollar amount")]
        public decimal TransactionAmount { get; set; } = decimal.Zero;




        public void Copy(IMoneyTransaction cpy)
        {
            if (cpy is null) return;

            this.UID = cpy.UID;
            this.TransactionDate = cpy.TransactionDate;
            this.JournalEntryType = cpy.JournalEntryType;
            this.DebitAccount = cpy.DebitAccount;
            this.DebitBankDate = cpy.DebitBankDate;
            this.CreditAccount = cpy.CreditAccount;
            this.CreditBankDate = cpy.CreditBankDate;
            this.Description = cpy.Description;
            this.TransactionAmount = cpy.TransactionAmount;
        }

        public void BuildFromPlan(IBudgetPlan plan)
        {
            if (plan is null) return;

            switch(plan.PlanType)
            {
                case BudgetPlanType.DebtPayment:
                    this.JournalEntryType = TransactionType.DebtPayment;
                    break;
                case BudgetPlanType.Payable:
                    this.JournalEntryType = TransactionType.Expense;
                    break;
                case BudgetPlanType.Receivable:
                    this.JournalEntryType = TransactionType.Income;
                    break;
                case BudgetPlanType.Transfer:
                    this.JournalEntryType = TransactionType.Transfer;
                    break;
                default:
                    throw new InvalidOperationException($"Budget Plan type {plan.PlanType.ToString()} is not supported");

            }

            this.Description = plan.Description;
            this.DebitAccount = plan.DebitAccount;
            this.CreditAccount = plan.CreditAccount;
            this.TransactionAmount = plan.ExpectedAmount;
        }
    }
}
