using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models.Summary
{
    public class SummaryListingVM
    {
        private readonly IServiceProvider provider;
        private readonly IGetMoneyAccountsUseCase getMoneyAccountsUseCase;

        public SummaryListingVM(IServiceProvider provider, IGetMoneyAccountsUseCase getMoneyAccountsUseCase)
        {
            this.provider = provider;
            this.getMoneyAccountsUseCase = getMoneyAccountsUseCase;
            this.Load();
        }


        public List<SummaryItemVM> SummaryItemList { get; set; } = new List<SummaryItemVM>();

        public void Load()
        {
            this.SummaryItemList.Clear();

            var listAccounts = getMoneyAccountsUseCase.Execute(false);
            foreach (var act in listAccounts.OrderBy(o => o.OrderBy).ThenBy(o => o.Description))
            {
                SummaryItemVM summary = provider.GetService<SummaryItemVM>();
                summary.LoadAccount(act);
                this.SummaryItemList.Add(summary);
            }
        }



    }
}
