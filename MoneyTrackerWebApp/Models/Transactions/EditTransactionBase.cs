using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.AspNetCore.Components;
using MoneyTrackerWebApp.Models.Core;
using MoneyTrackerWebApp.Services;
using System.Runtime.CompilerServices;

namespace MoneyTrackerWebApp.Models.Transactions
{
    public class EditTransactionBase : ComponentBase, IEditComponentBase
    {
        [Parameter]
        public Guid? TransactionId { get; set; }

        [Parameter]
        public string? TransType { get; set; }

        [Inject]
        public INavigationHistoryService Navigation { get; set; }

        [Inject]
        public StorageService<IMoneyTransaction> Storage { get; set; }

        [Inject]
        public IGetTransactionByIdUseCase ActionGetTransaction { get; set; }

        [Inject]
        public IGetJournalAccountListByTypesUseCase ActionGetAccountListByTypes { get; set; }

        [Inject]
        public IGetJournalAccountByUIDUseCase ActionGetAccountById { get; set; }

        [Inject]
        public ISaveTransactionUseCase ActionSave { get; set; }


        public string DebitLabel { get; set; } = "Debit";
        public string CreditLabel { get; set; } = "Credit";
        public bool EnableDebitBankDate { get; set; } = false;
        public bool EnableCreditBankDate { get; set; } = false;




        protected EditTransactionVM viewModel = new EditTransactionVM();
        protected List<IJournalAccount> listValidDebits = new List<IJournalAccount>();
        protected List<IJournalAccount> listValidCredits = new List<IJournalAccount>();




        protected override void OnParametersSet()
        {
            if (!TransactionId.HasValue && string.IsNullOrWhiteSpace(TransType)) throw new InvalidOperationException("Transaction editor cannot be configured");
                        
            if (TransactionId != null && TransactionId != Guid.Empty)
            {
                var trans = ActionGetTransaction.Execute(this.TransactionId.Value);
                Storage.Data = trans;
                viewModel.Copy(trans);
            }
            else
            {
                viewModel.JournalEntryType = TransType.ToTransType();
            }

            this.LoadDebitsAndCredits();
        }

        protected override void OnInitialized()
        {
            if(Storage.Data != null)
            {
                viewModel.Copy(Storage.Data);
                this.LoadDebitsAndCredits();
            }
        }


        #region Form Events

        protected void OnTransactionDateChanged(ChangeEventArgs e)
        {
            if(DateTime.TryParse(e.Value.ToString(), out DateTime newDate))
            {
                viewModel.TransactionDate = newDate;
            }
        }

        protected void OnDebitAccountChanged(ChangeEventArgs e)
        {
            Guid uid = Guid.Parse(e.Value.ToString());
            IJournalAccount account = ActionGetAccountById.Execute(uid);
            if(account != null)
            {
                viewModel.DebitAccount = account;
            }
        }

        protected void OnDebitBankDateChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Value?.ToString()))  {
                viewModel.DebitBankDate = null;
            } else
            {
            viewModel.DebitBankDate = DateTime.Parse(e.Value.ToString());
            }
        }

        protected void OnCreditAccountChanged(ChangeEventArgs e)
        {
            if (e.Value is null) return;
            Guid uid = Guid.Parse(e.Value.ToString());
            IJournalAccount account = ActionGetAccountById.Execute(uid);
            if (account != null)
            {
                viewModel.CreditAccount = account;
            }
        }

        protected void OnCreditBankDateChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
            {
                viewModel.CreditBankDate = null;
            }
            else
            {
                viewModel.CreditBankDate = DateTime.Parse(e.Value.ToString());
            }
        }

        protected void OnDescriptionChanged(ChangeEventArgs e)
        {
            viewModel.Description = e.Value?.ToString() ?? string.Empty;
        }

        protected void OnAmountChanged(ChangeEventArgs e)
        {
            if(decimal.TryParse(e.Value?.ToString() ?? string.Empty, out decimal newAmount))
            {
                viewModel.TransactionAmount = newAmount;
            }
        }

        #endregion



        protected void LoadDebitsAndCredits()
        {
            listValidCredits.Clear();
            listValidDebits.Clear();

            LedgerType[] listDebitTypes, listCreditTypes;
            switch(viewModel.JournalEntryType)
            {
                case TransactionType.Income:
                    listDebitTypes = [LedgerType.Bank];
                    listCreditTypes = [LedgerType.Receivable];
                    this.EnableDebitBankDate = true;
                    this.DebitLabel = "Bank";
                    this.CreditLabel = "Receivable";
                    break;
                case TransactionType.Expense:
                    listDebitTypes = [LedgerType.Payable];
                    listCreditTypes = [LedgerType.Bank, LedgerType.LiabilityCard];
                    this.EnableCreditBankDate = true;
                    this.DebitLabel = "Payable";
                    this.CreditLabel = "Money";
                    break;
                case TransactionType.DebtPayment:
                    listDebitTypes = [LedgerType.LiabilityCard, LedgerType.LiabilityLoan];
                    listCreditTypes = [LedgerType.Bank];
                    this.EnableCreditBankDate = true;
                    this.EnableDebitBankDate = true;
                    this.DebitLabel = "Liabiility";
                    this.CreditLabel = "Bank";
                    break;
                case TransactionType.DebtAdjustment:
                    listDebitTypes = [LedgerType.LiabilityCard, LedgerType.LiabilityLoan];
                    listCreditTypes = [];
                    listValidCredits.AddRange([SpecialAccount.DebtInterest, SpecialAccount.DebtReduction]);
                    this.EnableDebitBankDate = true;
                    this.DebitLabel = "Liability";
                    this.CreditLabel = "Action";
                    break;
                case TransactionType.Transfer:
                    listDebitTypes = [LedgerType.Bank];
                    listCreditTypes = [LedgerType.Bank];
                    this.EnableCreditBankDate = true;
                    this.EnableDebitBankDate = true;
                    this.DebitLabel = "Bank";
                    this.CreditLabel = "Bank";
                    break;
                case TransactionType.Correction:
                    listDebitTypes = [LedgerType.Bank, LedgerType.LiabilityCard, LedgerType.LiabilityLoan, LedgerType.Payable, LedgerType.Receivable];
                    listCreditTypes = [];
                    listValidCredits.Add(SpecialAccount.UnlistedAdjusment);
                    this.EnableDebitBankDate = true;
                    this.DebitLabel = "Account";
                    this.CreditLabel = "Action";
                    break;
                default: // Not Set
                    throw new InvalidOperationException("Transaction type has not been declared");
            }


            var listDebits = ActionGetAccountListByTypes.Execute(listDebitTypes.ToList());
            if(listDebits?.Any() == true)
            {
                listValidDebits.AddRange(listDebits);
            }

            var listCredits = ActionGetAccountListByTypes.Execute(listCreditTypes.ToList());
            if(listCredits?.Any() == true)
            {
                listValidCredits.AddRange(listCredits);
            }

        }

        public void SaveChanges()
        {
            ActionSave.Execute(viewModel);
            this.ReturnToList();
        }

        public void Reset()
        {
            if(Storage.Data != null)
            {
                viewModel.Copy(Storage.Data);
            } else
            {
                this.ReturnToList();
            }
        }

        private readonly string URL_TRANSLIST = "/transactions";
        public void ReturnToList()
        {
            Storage.Data = null;
            Navigation.NavigateBack(URL_TRANSLIST);
        }



    }

    internal static class TransactionOverrides
    {

        internal static TransactionType ToTransType(this int n)
        {
            switch(n)
            {
                case 1: return TransactionType.Income;
                case 2: return TransactionType.Expense;
                case 3: return TransactionType.DebtPayment;
                case 4: return TransactionType.DebtAdjustment;
                case 5: return TransactionType.Transfer;
                case 6: return TransactionType.Correction;
                default: return TransactionType.NotSet;
            }
        }

        internal static TransactionType ToTransType(this string n)
        {
            switch (n)
            {
                case "1": return TransactionType.Income;
                case "2": return TransactionType.Expense;
                case "3": return TransactionType.DebtPayment;
                case "4": return TransactionType.DebtAdjustment;
                case "5": return TransactionType.Transfer;
                case "6": return TransactionType.Correction;
                default: return TransactionType.NotSet;
            }
        }

        internal static int ToParameter(this TransactionType type)
        {
            switch(type)
            {
                case TransactionType.Income: return 1;
                case TransactionType.Expense: return 2;
                case TransactionType.DebtPayment: return 3;
                case TransactionType.DebtAdjustment: return 4;
                case TransactionType.Transfer: return 5;
                case TransactionType.Correction: return 6;
                default: return -1;
            }
        }

    }
}
