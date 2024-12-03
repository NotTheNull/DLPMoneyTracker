using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace MoneyTrackerWebApp.Services
{
    public interface IJournalAccountService
    {
        IJournalAccount GetAccount(Guid uid);
        void SaveAccount(IJournalAccount account);
    }

    public class JournalAccountService : IJournalAccountService
    {
        private readonly IGetJournalAccountByUIDUseCase getAccountByUIDUseCase;
        private readonly ISaveJournalAccountUseCase saveAccountUseCase;
        private readonly JournalAccountFactory accountFactory;

        public JournalAccountService(
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveJournalAccountUseCase saveAccountUseCase,
            JournalAccountFactory accountFactory)
        {
            this.getAccountByUIDUseCase = getAccountByUIDUseCase;
            this.saveAccountUseCase = saveAccountUseCase;
            this.accountFactory = accountFactory;
        }


        public IJournalAccount GetAccount(Guid uid)
        {
            return getAccountByUIDUseCase.Execute(uid);
        }

        public void SaveAccount(IJournalAccount account)
        {
            var saveAccount = getAccountByUIDUseCase.Execute(account.Id);
            if (saveAccount != null)
            {
                saveAccount.Copy(account);
            }
            else
            {
                saveAccount = accountFactory.Build(account);
            }

            saveAccountUseCase.Execute(saveAccount);
        }


    }
}
