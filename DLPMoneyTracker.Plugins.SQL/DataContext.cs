using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace DLPMoneyTracker.Plugins.SQL
{
    public class DataContext(IDLPConfig config) : DbContext()
    {
        private readonly IDLPConfig _config = config;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.DBConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Account>()
                .HasOne(x => x.SummaryAccount)
                .WithMany(x => x.SubLedgers)
                .HasForeignKey(x => x.SummaryAccountId);


            modelBuilder.Entity<BankReconciliation>()
                .HasOne(x => x.BankAccount)
                .WithMany(x => x.Reconciliations)
                .HasForeignKey(x => x.BankAccountId)
                .IsRequired();

            modelBuilder.Entity<BankReconciliation>().Navigation(br => br.BankAccount).AutoInclude();

            modelBuilder.Entity<BudgetPlan>()
                .HasOne(o => o.Debit)
                .WithMany(o => o.DebitBudgetPlans)
                .HasForeignKey(x => x.DebitId)
                .IsRequired();

            modelBuilder.Entity<BudgetPlan>()
                .HasOne(o => o.Credit)
                .WithMany(o => o.CreditBudgetPlans)
                .HasForeignKey(x => x.CreditId)
                .IsRequired();

            modelBuilder.Entity<BudgetPlan>().Navigation(p => p.Debit).AutoInclude();
            modelBuilder.Entity<BudgetPlan>().Navigation(p => p.Credit).AutoInclude();

            modelBuilder.Entity<CSVMain>()
                .HasOne(x => x.Account)
                .WithOne(x => x.CSVMapping)
                .HasForeignKey<CSVMain>(x => x.AccountId)
                .IsRequired();

            modelBuilder.Entity<CSVColumn>()
                .HasOne(x => x.Main)
                .WithMany(x => x.Columns)
                .HasForeignKey(x => x.MainId)
                .IsRequired();
            modelBuilder.Entity<CSVMain>().Navigation(p => p.Columns).AutoInclude();

            modelBuilder.Entity<TransactionDetail>()
                .HasOne(x => x.Batch)
                .WithMany(x => x.Details)
                .HasForeignKey(x => x.BatchId)
                .IsRequired();
            modelBuilder.Entity<TransactionDetail>()
                .HasOne(x => x.LedgerAccount)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.LedgerAccountId)
                .IsRequired();

            modelBuilder.Entity<TransactionBatch>().Navigation(b => b.Details).AutoInclude();
            modelBuilder.Entity<TransactionDetail>().Navigation(t => t.LedgerAccount).AutoInclude();

            
        }




        public DbSet<Account> Accounts { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BankReconciliation> Reconciliations { get; set; }
        public DbSet<TransactionBatch> TransactionBatches { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<CSVMain> CSVMains { get; set; }
        public DbSet<CSVColumn> CSVColumns { get; set; }


    }
}
