using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public class TransactionDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; } = 0;
        public int BatchId { get; set; }
        public int LedgerAccountId { get; set; }
        public decimal Amount { get; set; } = decimal.Zero;
        public DateTime? BankReconciliationDate { get; set; }

        public virtual Account LedgerAccount { get; set; }
        public virtual TransactionBatch Batch { get; set; }
    }
}
