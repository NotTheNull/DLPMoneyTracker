using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace DLPMoneyTracker.Plugins.SQL
{
    public class DataContext : DbContext
    {
        //private const string CONNECTION = @"server=DLP-HOME-PC\SQLEXPRESS; database=MoneyTracker; Trusted_Connection=True; TrustServerCertificate=True";
        private readonly IDLPConfig _config;

        public DataContext(IDLPConfig config) : base() 
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.SQLConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(x => x.Id);

            modelBuilder.Entity<BudgetPlan>().HasKey(x => x.Id);
            modelBuilder.Entity<BudgetPlan>().Navigation(p => p.Debit).AutoInclude();
            modelBuilder.Entity<BudgetPlan>().Navigation(p => p.Credit).AutoInclude();

            modelBuilder.Entity<BankReconciliation>().HasKey(x => x.Id);
            modelBuilder.Entity<BankReconciliation>().Navigation(br => br.BankAccount).AutoInclude();

            modelBuilder.Entity<TransactionBatch>().HasKey(x => x.Id);
            modelBuilder.Entity<TransactionBatch>().Navigation(b => b.Details).AutoInclude();

            modelBuilder.Entity<TransactionDetail>().HasKey(x => x.Id);
            modelBuilder.Entity<TransactionDetail>().Navigation(t => t.LedgerAccount).AutoInclude();
        }




        public DbSet<Account> Accounts { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BankReconciliation> Reconciliations { get; set; }
        public DbSet<TransactionBatch> TransactionBatches { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }


    }
}
