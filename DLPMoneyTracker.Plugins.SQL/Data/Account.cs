using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public enum LedgerType
    {
        Bank,
        LiabilityCard,
        LiabilityLoan,
        Receivable,
        Payable,
        NotSet
    }

    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid AccountUID { get; set; }
        [Required, StringLength(50)]
        public string Description { get; set; } = string.Empty;
        public LedgerType AccountType { get; set; }
        public int MainTabSortingId { get; set; } // Helps determine priority sorting when displaying the accounts on the main tab
        public DateTime? DateClosedUTC { get; set; }

    }
}
