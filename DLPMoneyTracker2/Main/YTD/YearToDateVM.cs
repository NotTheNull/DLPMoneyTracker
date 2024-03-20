
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.YTD
{
    public class YearToDateVM : BaseViewModel
    {
        // Plan here is to break down each Payables and Receivables by month with a YTD total in a data grid
        // Will need to have a Drill Down option to get the list of transactions for a given month

        private readonly int _year; // Exists for if we decide to use this UI for History
        private readonly IGetNominalAccountsUseCase getAccountsUseCase;
        private readonly NotificationSystem notifications;

        public YearToDateVM(
            IGetNominalAccountsUseCase getAccountsUseCase,
            NotificationSystem notifications)
        {
            this.getAccountsUseCase = getAccountsUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;

            _year = DateTime.Today.Year;

            IncomeAccountDetailList = new ObservableCollection<YTDAccountDetailVM>();
            ExpenseAccountDetailList = new ObservableCollection<YTDAccountDetailVM>();
            

            this.Load();
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            this.Load();
        }


        public ObservableCollection<YTDAccountDetailVM> IncomeAccountDetailList { get; set; }
        public ObservableCollection<YTDAccountDetailVM> ExpenseAccountDetailList { get; set; }

        private void Load()
        {
            IncomeAccountDetailList.Clear();
            ExpenseAccountDetailList.Clear();

            var listAccounts = getAccountsUseCase.Execute(true);
            if (listAccounts?.Any() != true) return;

            foreach (var act in listAccounts)
            {
                YTDAccountDetailVM vm = UICore.DependencyHost.GetRequiredService<YTDAccountDetailVM>();
                vm.LoadData(act, _year);

                if (act.JournalType == LedgerType.Receivable)
                {
                    IncomeAccountDetailList.Add(vm);
                }
                else
                {
                    ExpenseAccountDetailList.Add(vm);
                }
            }
        }
    }
}