using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetJournalAccountByUIDUseCase : IGetJournalAccountByUIDUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetJournalAccountByUIDUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public IJournalAccount Execute(Guid uid)
        {
            return accountRepository.GetAccountByUID(uid);
        }
    }
}
