﻿// <auto-generated />
using System;
using DLPMoneyTracker.Plugins.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DLPMoneyTracker.Plugins.SQL.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountType")
                        .HasColumnType("int");

                    b.Property<Guid>("AccountUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BudgetType")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateClosedUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("MainTabSortingId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.BankReconciliation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BankAccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("EndingBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("EndingDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("StartingBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("StartingDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BankAccountId");

                    b.ToTable("Reconciliations");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.BudgetPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CreditId")
                        .HasColumnType("int");

                    b.Property<int?>("DebitId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("ExpectedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Frequency")
                        .HasColumnType("int");

                    b.Property<int>("PlanType")
                        .HasColumnType("int");

                    b.Property<Guid>("PlanUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreditId");

                    b.HasIndex("DebitId");

                    b.ToTable("BudgetPlans");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.TransactionBatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BatchType")
                        .HasColumnType("int");

                    b.Property<Guid>("BatchUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("EnteredDateUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TransactionBatches");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.TransactionDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("BankReconciliationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("BatchId")
                        .HasColumnType("int");

                    b.Property<int?>("LedgerAccountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BatchId");

                    b.HasIndex("LedgerAccountId");

                    b.ToTable("TransactionDetails");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.BankReconciliation", b =>
                {
                    b.HasOne("DLPMoneyTracker.Plugins.SQL.Data.Account", "BankAccount")
                        .WithMany()
                        .HasForeignKey("BankAccountId");

                    b.Navigation("BankAccount");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.BudgetPlan", b =>
                {
                    b.HasOne("DLPMoneyTracker.Plugins.SQL.Data.Account", "Credit")
                        .WithMany()
                        .HasForeignKey("CreditId");

                    b.HasOne("DLPMoneyTracker.Plugins.SQL.Data.Account", "Debit")
                        .WithMany()
                        .HasForeignKey("DebitId");

                    b.Navigation("Credit");

                    b.Navigation("Debit");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.TransactionDetail", b =>
                {
                    b.HasOne("DLPMoneyTracker.Plugins.SQL.Data.TransactionBatch", "Batch")
                        .WithMany("Details")
                        .HasForeignKey("BatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DLPMoneyTracker.Plugins.SQL.Data.Account", "LedgerAccount")
                        .WithMany()
                        .HasForeignKey("LedgerAccountId");

                    b.Navigation("Batch");

                    b.Navigation("LedgerAccount");
                });

            modelBuilder.Entity("DLPMoneyTracker.Plugins.SQL.Data.TransactionBatch", b =>
                {
                    b.Navigation("Details");
                });
#pragma warning restore 612, 618
        }
    }
}
