using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace DLPMoneyTracker.BusinessLogic.AdapterInterfaces
{
    // T is the Source class
    public interface ISourceToJournalAccountAdapter<T> : IMoneyAccount, INominalAccount, ILiabilityAccount, ISubLedgerAccount, IDLPAdapter<T>
    {
    }
}