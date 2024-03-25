using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts
{
    public class GetNextCategoryIdUseCase : IGetNextCategoryIdUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetNextCategoryIdUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public int Execute()
        {
            return accountRepository.GetNextCategoryId();
        }
    }
}
