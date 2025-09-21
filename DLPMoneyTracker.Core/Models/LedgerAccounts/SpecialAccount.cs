using System.Diagnostics;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts;

[DebuggerDisplay("[{Id}] {JournalType} {Description}")]
public class SpecialAccount : IJournalAccount
{
    public static SpecialAccount InitialBalance => new() { Id = new Guid("11111111-1111-1111-1111-111111111111"), Description = "*STARTING BALANCE*" };
    public static SpecialAccount UnlistedAdjustment => new() { Id = new Guid("99999999-8888-7777-6666-555555555555"), Description = "*CORRECTION*" };
    public static SpecialAccount DebtInterest => new() { Id = new Guid("DDDDDDDD-EEEE-BBBB-FFFF-999999999999"), Description = "*INTEREST ACCRUES*" };
    public static SpecialAccount DebtReduction => new() { Id = new Guid("FFFFFFFF-0000-0000-0000-999999999999"), Description = "*DEBT REDUCTION*" };
    public static SpecialAccount InvalidAccount => new() { Id = new Guid("00000000-0000-0000-0000-000000000001"), Description = "*INVALID ACCOUNT*" };

    public SpecialAccount()    { }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Description { get; set; } = string.Empty;

    public LedgerType JournalType => LedgerType.Special;

    public DateTime? DateClosedUTC
    {
        get => null; set { }
    }

    public int OrderBy => 0;

    public void Copy(IJournalAccount cpy)
    {
        if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Journal Types do not match");

        this.Id = cpy.Id;
        this.Description = cpy.Description;
    }
}