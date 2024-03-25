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
    public class GetNextSubLedgerIdUseCase : IGetNextSubLedgerIdUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetNextSubLedgerIdUseCase(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }


        public int Execute(LedgerType accountType, int idCategory)
        {
            return accountRepository.GetNextSubLedgerId(accountType, idCategory);
        }
    }
}
