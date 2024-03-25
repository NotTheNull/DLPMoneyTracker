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
    public class GetJournalAccountByLedgerNumber : IGetJournalAccountByLedgerNumberUseCase
    {
        private readonly ILedgerAccountRepository accountRepository;

        public GetJournalAccountByLedgerNumber(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }


        public IJournalAccount Execute(string idCPA)
        {
            return accountRepository.GetAccountByLedgerNumber(idCPA);
        }
    }
}
