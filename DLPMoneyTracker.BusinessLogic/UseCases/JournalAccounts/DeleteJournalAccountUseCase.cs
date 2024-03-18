using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class DeleteJournalAccountUseCase : IDeleteJournalAccountUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public DeleteJournalAccountUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public void Execute(Guid accountUID)
        {
            var account = accountRepository.GetAccountByUID(accountUID);
            if (account is null) return;
            if (account.DateClosedUTC.HasValue) return;

            account.DateClosedUTC = DateTime.UtcNow;
            accountRepository.SaveJournalAccount(account);
        }
    }
}
