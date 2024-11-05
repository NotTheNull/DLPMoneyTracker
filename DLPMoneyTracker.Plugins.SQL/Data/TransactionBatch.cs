using DLPMoneyTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{


    public class TransactionBatch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public Guid BatchUID { get; set; } = Guid.NewGuid();
        public DateTime EnteredDateUTC { get; set; } = DateTime.UtcNow; // Should not be set by user
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public TransactionType BatchType { get; set; } = TransactionType.NotSet;

        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<TransactionDetail> Details { get; set; } = new List<TransactionDetail>();

    }
}
