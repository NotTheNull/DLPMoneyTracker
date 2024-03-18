using DLPMoneyTracker.Plugins.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace DLPMoneyTracker.Plugins.SQL
{
    public class DataContext : DbContext
    {
        private const string CONNECTION = @"server=DLP-HOME-PC\SQLEXPRESS; database=MoneyTracker; Trusted_Connection=True; TrustServerCertificate=True";
        public DataContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CONNECTION);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(x => x.Id);
            modelBuilder.Entity<BudgetPlan>().HasKey(x => x.Id);
            modelBuilder.Entity<BankReconciliation>().HasKey(x => x.Id);
            modelBuilder.Entity<TransactionBatch>().HasKey(x => x.Id);
            modelBuilder.Entity<TransactionDetail>().HasKey(x => x.Id);
        }




        public DbSet<Account> Accounts { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BankReconciliation> Reconciliations { get; set; }
        public DbSet<TransactionBatch> TransactionBatches { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }


    }
}
