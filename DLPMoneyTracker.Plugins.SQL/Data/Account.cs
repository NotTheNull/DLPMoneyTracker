using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public Guid AccountUID { get; set; } = Guid.NewGuid();

        [Required, StringLength(50)]
        public string Description { get; set; } = string.Empty;
        public LedgerType AccountType { get; set; } = LedgerType.NotSet;
        public int MainTabSortingId { get; set; } = 0; // Helps determine priority sorting when displaying the accounts on the main tab
        public DateTime? DateClosedUTC { get; set; } = null;

        // For Nominal Accounts
        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK;
        public decimal DefaultBudget { get; set; } = decimal.Zero;
        public decimal CurrentBudget { get; set; } = decimal.Zero;
        public int? SummaryAccountId { get; set; }


        public virtual Account? SummaryAccount { get; set; }
        public virtual CSVMain CSVMapping { get; set; }
        public virtual ICollection<Account> SubLedgers { get; set; } 
        public virtual ICollection<BudgetPlan> DebitBudgetPlans { get; set; }
        public virtual ICollection<BudgetPlan> CreditBudgetPlans { get; set; }
        public virtual ICollection<TransactionDetail> Transactions { get; set; }
        public virtual ICollection<BankReconciliation> Reconciliations { get; set; }
    }
}
