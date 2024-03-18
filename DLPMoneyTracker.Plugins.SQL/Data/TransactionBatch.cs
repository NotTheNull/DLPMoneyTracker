using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public enum TransactionType
    {
        NotSet,
        Expense,
        Income,
        DebtPayment,
        DebtAdjustment,
        Correction,
        Transfer
    }

    public class TransactionBatch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime EnteredDateUTC { get; set; } = DateTime.UtcNow; // Should not be set by user
        public DateTime TransactionDate { get; set; } = DateTime.Today; 
        public TransactionType BatchType { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

    }
}
