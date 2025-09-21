using System.Diagnostics;

namespace DLPMoneyTracker.Core.Models.LedgerAccounts;

[DebuggerDisplay("[{Id}] {JournalType} {Description}")]
public class CreditCardAccount : IMoneyAccount, ILiabilityAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Description { get; set; } = string.Empty;

    public LedgerType JournalType => LedgerType.LiabilityCard;

    public int OrderBy { get; set; } = 0;

    public DateTime? DateClosedUTC { get; set; }

    public ICSVMapping Mapping { get; } = new CSVMapping();

    public CreditCardAccount() { }

    public CreditCardAccount(IJournalAccount cpy) => this.Copy(cpy);

    public void Copy(IJournalAccount cpy)
    {
        if (cpy.JournalType != this.JournalType) throw new InvalidOperationException("Copy MUST be a Credit Card Account");

        this.Id = cpy.Id;
        this.Description = cpy.Description;
        this.OrderBy = cpy.OrderBy;
        this.DateClosedUTC = cpy.DateClosedUTC;

        if (cpy is IMoneyAccount money && money.Mapping != null)
        {
            this.Mapping.Copy(money.Mapping);
        }
    }
}