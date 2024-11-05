using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Data
{
    public class CSVMain
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int StartingRow { get; set; } = 2;
        public bool IsAmountInverted { get; set; } = false;

        public virtual Account Account { get; set; }
        public virtual ICollection<CSVColumn> Columns { get; set; } = new List<CSVColumn>();
    }

    public class CSVColumn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int MainId { get; set; }

        [Required, StringLength(200)]
        public string ColumnName { get; set; } = string.Empty;
        public int ColumnIndex { get; set; } = 1;

        public virtual CSVMain Main { get; set; }

    }
}
