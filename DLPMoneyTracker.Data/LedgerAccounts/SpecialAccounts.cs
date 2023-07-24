using DLPMoneyTracker.Data.ConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.LedgerAccounts
{
    public class SpecialAccount : IJournalAccount
    {
        public static SpecialAccount InitialBalance { get { return new SpecialAccount() { Id = Guid.Empty, Description = "*STARTING BALANCE*" }; } }
        public static SpecialAccount UnlistedAdjusment { get { return new SpecialAccount() { Id = new Guid("99999999-8888-7777-6666-555555555555"), Description = "*CORRECTION*" }; } }
        public static SpecialAccount DebtInterest { get { return new SpecialAccount() { Id = new Guid("DDDDDDDD-EEEE-BBBB-FFFF-999999999999"), Description = "*INTEREST ACCRUES*" }; } }
        public static SpecialAccount DebtReduction { get { return new SpecialAccount() { Id = new Guid("FFFFFFFF-0000-0000-0000-999999999999"), Description = "*DEBT REDUCTION*" }; } }


        public Guid Id { get; set; }

        public string Description { get; set; }

        public JournalAccountType JournalType { get { return JournalAccountType.NotSet; } }

        public DateTime? DateClosedUTC { get { return null; } set { } }

        public int OrderBy { get { return 0; } }

        public string MoneyAccountId { get { return string.Empty; } }

        public MoneyAccountType AccountType { get { return MoneyAccountType.NotSet; } }

        public Guid CategoryId { get { return Guid.Empty; } }

        public decimal MonthlyBudgetAmount { get { return decimal.Zero; } }

        public bool ExcludeFromBudget { get { return true; } }


        public void Copy(IJournalAccount cpy)
        {
            return;
        }
    }
}
