﻿using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Plugins.JSON.Repositories;
using DLPMoneyTracker.Plugins.SQL.Repositories;
using DLPMoneyTracker2.Core;
using System;
using System.Linq;
using System.Windows;

namespace DLPMoneyTracker2.Conversion
{
    public class JSONConversionVM : BaseViewModel
    {
        private readonly JSONLedgerAccountRepository jsonAccountRepository;
        private readonly JSONBudgetPlanRepository jsonPlanRepository;
        private readonly JSONBankReconciliationRepository jsonReconciliationRepository;
        private readonly JSONTransactionRepository jsonTransactionRepository;
        private readonly SQLLedgerAccountRepository sqlAccountRepository;
        private readonly SQLBudgetPlanRepository sqlPlanRepository;
        private readonly SQLBankReconciliationRepository sqlReconciliationRepository;
        private readonly SQLTransactionRepository sqlTransactionRepository;

        public JSONConversionVM(
            JSONLedgerAccountRepository jsonAccountRepository,
            JSONBudgetPlanRepository jsonPlanRepository,
            JSONBankReconciliationRepository jsonReconciliationRepository,
            JSONTransactionRepository jsonTransactionRepository,
            SQLLedgerAccountRepository sqlAccountRepository,
            SQLBudgetPlanRepository sqlPlanRepository,
            SQLBankReconciliationRepository sqlReconciliationRepository,
            SQLTransactionRepository sqlTransactionRepository
            )
        {
            this.jsonAccountRepository = jsonAccountRepository;
            this.jsonPlanRepository = jsonPlanRepository;
            this.jsonReconciliationRepository = jsonReconciliationRepository;
            this.jsonTransactionRepository = jsonTransactionRepository;
            this.sqlAccountRepository = sqlAccountRepository;
            this.sqlPlanRepository = sqlPlanRepository;
            this.sqlReconciliationRepository = sqlReconciliationRepository;
            this.sqlTransactionRepository = sqlTransactionRepository;

            this.Refresh();
        }

        #region Properties

        private readonly ConversionDTO _jsonCounts = new();

        public int JSONAccountCount
        {
            get { return _jsonCounts.JournalAccountCount; }
            set
            {
                _jsonCounts.JournalAccountCount = value;
                NotifyPropertyChanged(nameof(JSONAccountCount));
            }
        }

        public int JSONPlanCount
        {
            get { return _jsonCounts.BudgetPlanCount; }
            set
            {
                _jsonCounts.BudgetPlanCount = value;
                NotifyPropertyChanged(nameof(JSONPlanCount));
            }
        }

        public int JSONReconciliationCount
        {
            get { return _jsonCounts.BankReconciliationCount; }
            set
            {
                _jsonCounts.BankReconciliationCount = value;
                NotifyPropertyChanged(nameof(JSONReconciliationCount));
            }
        }

        public long JSONTransactionCount
        {
            get { return _jsonCounts.TransactionCount; }
            set
            {
                _jsonCounts.TransactionCount = value;
                NotifyPropertyChanged(nameof(JSONTransactionCount));
            }
        }

        private readonly ConversionDTO _sqlCounts = new();

        public int SQLAccountCount
        {
            get { return _sqlCounts.JournalAccountCount; }
            set
            {
                _sqlCounts.JournalAccountCount = value;
                NotifyPropertyChanged(nameof(SQLAccountCount));
            }
        }

        public int SQLPlanCount
        {
            get { return _sqlCounts.BudgetPlanCount; }
            set
            {
                _sqlCounts.BudgetPlanCount = value;
                NotifyPropertyChanged(nameof(SQLPlanCount));
            }
        }

        public int SQLReconciliationCount
        {
            get { return _sqlCounts.BankReconciliationCount; }
            set
            {
                _sqlCounts.BankReconciliationCount = value;
                NotifyPropertyChanged(nameof(SQLReconciliationCount));
            }
        }

        public long SQLTransactionCount
        {
            get { return _sqlCounts.TransactionCount; }
            set
            {
                _sqlCounts.TransactionCount = value;
                NotifyPropertyChanged(nameof(SQLTransactionCount));
            }
        }

        #endregion Properties

        #region Commands

        public RelayCommand CommandRefresh => new((o) => this.Refresh());

        public RelayCommand CommandExportJSON => new((o) => this.ExportJSON());

        public RelayCommand CommandImportSQL => new((o) => this.ImportSQL());

        #endregion Commands

        #region UI Helpers

        private bool _showProgressBar;

        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set
            {
                _showProgressBar = value;
                NotifyPropertyChanged(nameof(ShowProgressBar));
            }
        }

        #endregion UI Helpers

        private void ExportJSON()
        {
            if (this.ShowProgressBar) return;

            this.ShowProgressBar = true;
            try
            {
                var listAccounts = sqlAccountRepository.GetFullList();
                jsonAccountRepository.AccountList.Clear();
                jsonAccountRepository.AccountList.AddRange(listAccounts);
                jsonAccountRepository.SaveToFile();

                var listBudgetPlans = sqlPlanRepository.GetFullList();
                jsonPlanRepository.BudgetPlanList.Clear();
                jsonPlanRepository.BudgetPlanList.AddRange(listBudgetPlans);
                jsonPlanRepository.SaveToFile();

                var listTransactions = sqlTransactionRepository.GetFullList();
                jsonTransactionRepository.TransactionList.Clear();
                jsonTransactionRepository.TransactionList.AddRange(listTransactions);
                jsonTransactionRepository.SaveToFile();

                var listReconciliations = sqlReconciliationRepository.GetFullList();
                jsonReconciliationRepository.BankReconciliationList.Clear();
                jsonReconciliationRepository.BankReconciliationList.AddRange(listReconciliations);
                jsonReconciliationRepository.SaveToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to export: {ex.Message}");
            }
            finally
            {
                this.Refresh();
                this.ShowProgressBar = false;
            }
        }

        private void ImportSQL()
        {
            if (this.ShowProgressBar) return; // Work already in progress

            this.ShowProgressBar = true;
            try
            {
                var listAccounts = jsonAccountRepository.GetFullList();
                foreach (var account in listAccounts)
                {
                    sqlAccountRepository.SaveJournalAccount(account);
                }

                var listBudgetPlans = jsonPlanRepository.GetFullList();
                foreach (var plan in listBudgetPlans)
                {
                    sqlPlanRepository.SavePlan(plan);
                }

                var listTransactions = jsonTransactionRepository.GetFullList();
                foreach (var record in listTransactions.OrderBy(o => o.TransactionDate).ThenBy(o => o.UID))
                {
                    sqlTransactionRepository.SaveTransaction(record);
                }

                var listReconciliations = jsonReconciliationRepository.GetFullList();
                foreach (var reconciliation in listReconciliations.SelectMany(s => s.ReconciliationList))
                {
                    sqlReconciliationRepository.SaveReconciliation(reconciliation);
                }

                MessageBox.Show("Import complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to import: {ex.Message}");
            }
            finally
            {
                this.Refresh();
                this.ShowProgressBar = false;
            }
        }

        private void Refresh()
        {
            this.JSONAccountCount = jsonAccountRepository.GetRecordCount();
            this.JSONPlanCount = jsonPlanRepository.GetRecordCount();
            this.JSONReconciliationCount = jsonReconciliationRepository.GetRecordCount();
            this.JSONTransactionCount = jsonTransactionRepository.GetRecordCount();
            this.SQLAccountCount = sqlAccountRepository.GetRecordCount();
            this.SQLPlanCount = sqlPlanRepository.GetRecordCount();
            this.SQLReconciliationCount = sqlReconciliationRepository.GetRecordCount();
            this.SQLTransactionCount = sqlTransactionRepository.GetRecordCount();

            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(JSONAccountCount));
            NotifyPropertyChanged(nameof(JSONPlanCount));
            NotifyPropertyChanged(nameof(JSONReconciliationCount));
            NotifyPropertyChanged(nameof(JSONTransactionCount));
            NotifyPropertyChanged(nameof(SQLAccountCount));
            NotifyPropertyChanged(nameof(SQLPlanCount));
            NotifyPropertyChanged(nameof(SQLReconciliationCount));
            NotifyPropertyChanged(nameof(SQLTransactionCount));
        }
    }
}