using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BankReconciliation;
using DLPMoneyTracker.Plugins.SQL.Adapters;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace DLPMoneyTracker.Plugins.SQL.Repositories
{
    public class SQLBankReconciliationRepository(NotificationSystem notification, IDLPConfig config, ILedgerAccountRepository accountRepository) : IBankReconciliationRepository
    {
        private readonly NotificationSystem notification = notification;
        private readonly IDLPConfig config = config;
        private readonly ILedgerAccountRepository accountRepository = accountRepository;

        public List<BankReconciliationOverviewDTO> GetFullList()
        {
            List<BankReconciliationOverviewDTO> listOverviews = [];
            using (DataContext context = new(config))
            {
                List<Account> listBankAccounts = [.. context.Reconciliations.Select(s => s.BankAccount).Distinct().Where(x => x != null)];
                if (listBankAccounts?.Any() != true) return listOverviews;

                foreach (Account bank in listBankAccounts)
                {
                    BankReconciliationOverviewDTO overview = new BankReconciliationOverviewDTO()
                    {
                        BankAccount = accountRepository.GetAccountByUID(bank.AccountUID)
                    };
                    listOverviews.Add(overview);

                    var listReconciliations = context.Reconciliations.Where(x => x.BankAccount != null && x.BankAccount.AccountUID == bank.AccountUID).ToList();
                    foreach (var record in listReconciliations)
                    {
                        BankReconciliationDTO dto = new()
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

        public List<IMoneyTransaction> GetReconciliationTransactions(Guid accountUID, DateRange statementDates)
        {
            List<IMoneyTransaction> listMoneyRecords = [];
            using (DataContext context = new(config))
            {
                var listTransactions = context.TransactionBatches
                    .Where(x =>
                        x.Details.Any(y =>
                            y.LedgerAccount != null &&
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

                SQLSourceToTransactionAdapter adapter = new(context, accountRepository);
                foreach (var record in listTransactions)
                {
                    adapter.ImportSource(record);
                    listMoneyRecords.Add(new MoneyTransaction(adapter));
                }
            }

            return listMoneyRecords;
        }

        public void SaveReconciliation(BankReconciliationDTO dto)
        {
            using (DataContext context = new(config))
            {
                var existingReconciliation = context.Reconciliations
                    .FirstOrDefault(x =>
                        x.BankAccount != null &&
                        x.BankAccount.AccountUID == dto.BankAccount.Id &&
                        x.StartingDate == dto.StatementDate.Begin &&
                        x.EndingDate == dto.StatementDate.End
                    );
                if (existingReconciliation is null)
                {
                    // Need to make sure that the date range doesn't intersect with an existing record
                    bool hasIntersectingDates = context.Reconciliations
                        .Any(x =>
                            x.BankAccount != null &&
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

                    existingReconciliation = new()
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
            using DataContext context = new(config);
            return context.Reconciliations.Count();
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