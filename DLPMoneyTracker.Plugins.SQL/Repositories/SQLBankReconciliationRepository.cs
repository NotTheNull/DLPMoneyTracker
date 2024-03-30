using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBankReconciliationRepository : IBankReconciliationRepository
    {
        private readonly NotificationSystem notification;

        public SQLBankReconciliationRepository(NotificationSystem notification)
        {
            this.notification = notification;
        }

        public List<BankReconciliationOverviewDTO> GetFullList()
        {
            List<BankReconciliationOverviewDTO> listOverviews = new List<BankReconciliationOverviewDTO>();
            using (DataContext context = new DataContext())
            {
                var listBankAccounts = context.Reconciliations.Select(s => s.BankAccount).Distinct();
                if (listBankAccounts?.Any() != null) return listOverviews;

                foreach(Account bank in listBankAccounts)
                {
                    BankReconciliationOverviewDTO overview = new BankReconciliationOverviewDTO()
                    {
                        BankAccount = SourceToAccount(bank)
                    };
                    listOverviews.Add(overview); 

                    var listReconciliations = context.Reconciliations.Where(x => x.BankAccount.AccountUID == bank.AccountUID).ToList();
                    foreach(var record in listReconciliations)
                    {
                        BankReconciliationDTO dto = new BankReconciliationDTO()
                        {
                            BankAccount = overview.BankAccount,
                            StatementDate = new DateRange(record.StartingDate, record.EndingDate),
                            StartingBalance = record.StartingBalance,
                            EndingBalance = record.EndingBalance
                        };
                        overview.ReconciliationList.Add(dto);
                    }

                }

            }

            return listOverviews;
        }

        private IJournalAccount SourceToAccount(Account src)
        {
            SQLSourceToJournalAccountAdapter adapter = new SQLSourceToJournalAccountAdapter();
            adapter.ImportSource(src);

            JournalAccountFactory factory = new JournalAccountFactory();
            return factory.Build(adapter);
        }

        public List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates)
        {
            List<IMoneyTransaction> listMoneyRecords = new List<IMoneyTransaction>();
            using (DataContext context = new DataContext())
            {
                var listTransactions = context.TransactionBatches
                    .Where(x =>
                        x.Details.Any(y =>
                            y.LedgerAccount.AccountUID == accountUID &&
                            (
                                !y.BankReconciliationDate.HasValue ||
                                (
                                    y.BankReconciliationDate.HasValue &&
                                    y.BankReconciliationDate >= statementDates.Begin &&
                                    y.BankReconciliationDate <= statementDates.End
                                )
                            )
                        )
                    )
                    .Include(x => x.Details)
                    .ToList();
                if (listTransactions?.Any() != true) return listMoneyRecords;

                SQLSourceToTransactionAdapter adapter = new SQLSourceToTransactionAdapter(context);
                foreach(var record in listTransactions)
                {
                    adapter.ImportSource(record);
                    listMoneyRecords.Add(new MoneyTransaction(adapter));
                }
            }

            return listMoneyRecords;
        }

        

        public void SaveReconciliation(BankReconciliationDTO dto)
        {
            using (DataContext context = new DataContext())
            {
                var existingReconciliation = context.Reconciliations
                    .FirstOrDefault(x =>
                        x.BankAccount.AccountUID == dto.BankAccount.Id &&
                        x.StartingDate == dto.StatementDate.Begin &&
                        x.EndingDate == dto.StatementDate.End
                    );
                if(existingReconciliation is null)
                {
                    // Need to make sure that the date range doesn't intersect with an existing record
                    bool hasIntersectingDates = context.Reconciliations
                        .Any(x =>
                            x.BankAccount.AccountUID == dto.BankAccount.Id &&
                            (
                                (
                                    x.StartingDate >= dto.StatementDate.Begin &&
                                    x.StartingDate <= dto.StatementDate.End
                                ) ||
                                (
                                    x.EndingDate >= dto.StatementDate.Begin &&
                                    x.EndingDate <= dto.StatementDate.End
                                )
                            )
                        );
                    if (hasIntersectingDates) throw new BadReconciliationException("Statement dates overlap existing reconciliations");

                    existingReconciliation = new BankReconciliation()
                    {
                        BankAccount = context.Accounts.FirstOrDefault(x => x.AccountUID == dto.BankAccount.Id),
                        StartingDate = dto.StatementDate.Begin,
                        EndingDate = dto.StatementDate.End,
                        StartingBalance = dto.StartingBalance,
                        EndingBalance = dto.EndingBalance
                    };
                    context.Reconciliations.Add(existingReconciliation);
                }
                else
                {
                    existingReconciliation.StartingBalance = dto.StartingBalance;
                    existingReconciliation.EndingBalance = dto.EndingBalance;
                }

                context.SaveChanges();
            }

            notification.TriggerBankReconciliationChanged(dto.BankAccount.Id);
        }

        public int GetRecordCount()
        {
            using (DataContext context = new DataContext())
            {
                return context.Reconciliations.Count();
            }
        }
    }

    public class BadReconciliationException : Exception
    {
        public BadReconciliationException()
        {
            
        }

        public BadReconciliationException(string? message) : base(message)
        {
        }

        public BadReconciliationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }


    }
}
