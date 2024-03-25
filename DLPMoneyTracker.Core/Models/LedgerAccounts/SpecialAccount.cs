
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts
{
    public class SpecialAccount : IJournalAccount
    {
        public static SpecialAccount InitialBalance { get { return new SpecialAccount() { Id = new Guid("11111111-1111-1111-111111111111"), Description = "*STARTING BALANCE*", CategoryId = 1, SubLedgerId = 0 }; } }
        public static SpecialAccount UnlistedAdjusment { get { return new SpecialAccount() { Id = new Guid("99999999-8888-7777-6666-555555555555"), Description = "*CORRECTION*", CategoryId = 2, SubLedgerId = 0 }; } }
        public static SpecialAccount DebtInterest { get { return new SpecialAccount() { Id = new Guid("DDDDDDDD-EEEE-BBBB-FFFF-999999999999"), Description = "*INTEREST ACCRUES*", CategoryId = 3, SubLedgerId = 0 }; } }
        public static SpecialAccount DebtReduction { get { return new SpecialAccount() { Id = new Guid("FFFFFFFF-0000-0000-0000-999999999999"), Description = "*DEBT REDUCTION*", CategoryId = 4, SubLedgerId = 0 }; } }


        public SpecialAccount()
        {

        }


        public Guid Id { get; set; }

        public string Description { get; set; }

        public LedgerType JournalType { get { return LedgerType.NotSet; } }

        public DateTime? DateClosedUTC { get { return null; } set { } }

        public int OrderBy { get { return 0; } }

        public string LedgerNumber { get { return string.Format("{0}-{1}-{2}", JournalType.ToLedgerNumber(), CategoryId, SubLedgerId); } }

        public int CategoryId { get; set; }

        public int SubLedgerId { get; set; }


        public void Copy(IJournalAccount cpy)
        {
            if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Journal Types do not match");

            this.Id = cpy.Id;
            this.Description = cpy.Description;
            this.CategoryId = cpy.CategoryId;
            this.SubLedgerId = cpy.SubLedgerId;
        }
    }
}
