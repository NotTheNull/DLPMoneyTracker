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
        public int Id { get; set; }
        public Guid AccountUID { get; set; } = Guid.NewGuid();

        [Required, StringLength(50)]
        public string Description { get; set; } = string.Empty;
        public LedgerType AccountType { get; set; } = LedgerType.NotSet;
        public int MainTabSortingId { get; set; } = 0; // Helps determine priority sorting when displaying the accounts on the main tab
        public DateTime? DateClosedUTC { get; set; }

        // For Nominal Accounts
        public BudgetTrackingType BudgetType { get; set; } = BudgetTrackingType.DO_NOT_TRACK; 

    }
}
