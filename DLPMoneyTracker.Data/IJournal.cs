﻿using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data
{
    public delegate void JournalModifiedHandler();

    public interface IJournal
    {
        event JournalModifiedHandler JournalModified;

        string FilePath { get; }
        ReadOnlyCollection<IJournalEntry> TransactionList { get; }

        void LoadFromFile(int year);
        void SaveToFile();
        void AddTransaction(IJournalEntry trans);

        decimal GetAccountBalance(Guid ledgerAccountId);
        decimal GetAccountBalance_CurrentMonth(Guid ledgerAccountId);
        decimal GetAccountBalance_Month(Guid ledgerAccountId, int year, int month);
        decimal GetAccountBalance_Range(Guid ledgerAccountId, DateTime beg, DateTime end);

#pragma warning disable CS0612 // Type or member is obsolete
        void Convert(ILedger ledger);
#pragma warning restore CS0612 // Type or member is obsolete
    }

    public class DLPJournal : IJournal
    {
        public event JournalModifiedHandler JournalModified;

        private readonly ITrackerConfig _config;
        private int _year;

        public DLPJournal(ITrackerConfig config) : this(config, DateTime.Today.Year) { }
        public DLPJournal(ITrackerConfig config, int year)
        {
            _config = config;
            this.LoadFromFile(year);
        }


        private string FolderPath { get { return AppConfigSettings.DATA_FOLDER_PATH.Replace(AppConfigSettings.YEAR_FOLDER_PLACEHOLDER, _year.ToString()); } }
        public string FilePath { get { return string.Concat(this.FolderPath, "Journal.json"); } }

        private List<IJournalEntry> _listTransactions = new List<IJournalEntry>();
        public ReadOnlyCollection<IJournalEntry> TransactionList { get { return _listTransactions.AsReadOnly(); } }




        public void AddTransaction(IJournalEntry trans)
        {
            var record = _listTransactions.FirstOrDefault(x => x.Id == trans.Id);
            if(record is null)
            {
                _listTransactions.Add(trans);
            }
            else
            {
                record.Copy(trans);
            }
            this.SaveToFile();
        }

        public decimal GetAccountBalance(Guid ledgerAccountId)
        {
            return GetAccountBalance_Range(ledgerAccountId, DateTime.MinValue, DateTime.MaxValue);
        }

        public decimal GetAccountBalance_CurrentMonth(Guid ledgerAccountId)
        {
            return GetAccountBalance_Month(ledgerAccountId, DateTime.Today.Year, DateTime.Today.Month);
        }

        public decimal GetAccountBalance_Month(Guid ledgerAccountId, int year, int month)
        {
            if (month < 1 || month > 12) throw new InvalidOperationException(String.Format("Month #{0} is not valid", month));

            DateTime beg = new DateTime(year, month, 1);
            int dayCount = DateTime.DaysInMonth(year, month);
            DateTime end = new DateTime(year, month, dayCount).AddDays(1);

            return GetAccountBalance_Range(ledgerAccountId, beg, end);
        }

        public decimal GetAccountBalance_Range(Guid ledgerAccountId, DateTime beg, DateTime end)
        {
            bool doesFilterApply(IJournalEntry entry)
            {
                return 
                    (
                        entry.DebitAccountId == ledgerAccountId 
                        || entry.CreditAccountId == ledgerAccountId
                    )
                    && entry.TransactionDate.Date >= beg.Date 
                    && entry.TransactionDate.Date <= end.Date;
            };

            decimal balance = decimal.Zero;
            if(_listTransactions.Any(doesFilterApply))
            {
                foreach(var t in _listTransactions.Where(doesFilterApply))
                {
                    if(t.DebitAccountId == ledgerAccountId)
                    {
                        balance += t.TransactionAmount;
                    }
                    else
                    {
                        balance -= t.TransactionAmount;
                    }
                }
            }

            return balance;
        }

        public void LoadFromFile(int year)
        {
            _year = year;
            if (!Directory.Exists(this.FolderPath))
            {
                Directory.CreateDirectory(this.FolderPath);
            }

            if (_listTransactions is null) _listTransactions = new List<IJournalEntry>();
            _listTransactions.Clear();
            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var dataList = (List<JournalEntryJSON>)JsonSerializer.Deserialize(json, typeof(List<JournalEntryJSON>));
            if (dataList?.Any() != true) return;

            foreach(var trans in dataList)
            {
                JournalEntry record = new JournalEntry(_config, trans);
                _listTransactions.Add(record);
            }
        }

        public void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_listTransactions);
            File.WriteAllText(FilePath, json);
            JournalModified?.Invoke();
        }

#pragma warning disable CS0612 // Type or member is obsolete
        const string XFER_CATEGORY = "*TRANSFER*";
        const string DEBT_CATEGORY = "*DEBT PAYMENT*";
        public void Convert(ILedger ledger)
        {
            if (ledger is null) return;

            _listTransactions.Clear();
            bool needNextRecord = false;
            JournalEntry newRecord = null;
            var ledgerList = ledger.TransactionList.ToList();
            foreach (var t in ledgerList)
            {
                // Find Money Account
                var money = _config.LedgerAccountsList.FirstOrDefault(x => x.AccountType != ConfigModels.MoneyAccountType.NotSet && x.MoneyAccountId == t.AccountID);


                var legacyCategory = _config.GetCategory(t.CategoryUID);
                if (legacyCategory.CategoryType == ConfigModels.CategoryType.InitialBalance)
                {
                    if (newRecord is null)
                    {
                        newRecord = new JournalEntry(_config)
                        {
                            TransactionDate = t.TransDate,
                            TransactionAmount = t.TransAmount,
                            Description = "*STARTING BALANCE*"
                        };
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Expected {0} record", newRecord.Description));
                    }

                    if (money is BankAccount bank)
                    {
                        newRecord.DebitAccount = bank;
                        newRecord.CreditAccount = SpecialAccount.InitialBalance;
                    }
                    else if (money is CreditCardAccount creditCard)
                    {
                        newRecord.DebitAccount = SpecialAccount.InitialBalance;
                        newRecord.CreditAccount = creditCard;
                    }
                    else if (money is LoanAccount loan)
                    {
                        newRecord.DebitAccount = SpecialAccount.InitialBalance;
                        newRecord.CreditAccount = loan;
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Money Account Type [{0}] not supported", money.GetType().FullName));
                    }


                }
                else if (legacyCategory.CategoryType == ConfigModels.CategoryType.Payment)
                {
                    if (newRecord is null)
                    {
                        newRecord = new JournalEntry(_config)
                        {
                            TransactionDate = t.TransDate,
                            TransactionAmount = t.TransAmount,
                            Description = DEBT_CATEGORY
                        };
                        needNextRecord = true;
                    }
                    else if (newRecord.Description != DEBT_CATEGORY)
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Expected {0} entry; Actual {1}", DEBT_CATEGORY, newRecord.Description));
                    }

                    if (money is BankAccount bank)
                    {
                        newRecord.CreditAccount = bank;
                    }
                    else if (money is CreditCardAccount creditCard)
                    {
                        newRecord.DebitAccount = creditCard;
                    }
                    else if (money is LoanAccount loan)
                    {
                        newRecord.DebitAccount = loan;
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Money Account Type [{0}] not supported for DEBT PAYMENT", money.GetType().FullName));
                    }

                    if (needNextRecord)
                    {
                        needNextRecord = false;
                        continue;
                    }
                }
                else if (legacyCategory.CategoryType == ConfigModels.CategoryType.TransferFrom || legacyCategory.CategoryType == ConfigModels.CategoryType.TransferTo)
                {
                    if (newRecord is null)
                    {
                        newRecord = new JournalEntry(_config)
                        {
                            TransactionDate = t.TransDate,
                            TransactionAmount = t.TransAmount,
                            Description = XFER_CATEGORY
                        };
                        needNextRecord = true;
                    }
                    else if (newRecord.Description != XFER_CATEGORY)
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Expected {0} entry; Actual {1}", XFER_CATEGORY, newRecord.Description));
                    }

                    if (money is BankAccount bank)
                    {
                        if (legacyCategory.CategoryType == ConfigModels.CategoryType.TransferFrom)
                        {
                            newRecord.CreditAccount = bank;
                        }
                        else
                        {
                            newRecord.DebitAccount = bank;
                        }
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Money Account Type [{0}] not supported for TRANSFER", money.GetType().FullName));
                    }

                    if (needNextRecord)
                    {
                        needNextRecord = false;
                        continue;
                    }
                }
                else
                {
                    if (newRecord is null)
                    {
                        newRecord = new JournalEntry(_config)
                        {
                            TransactionDate = t.TransDate,
                            TransactionAmount = t.TransAmount,
                            Description = t.Description
                        };
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Expected {0} record", newRecord.Description));
                    }


                    // Need to find new category account
                    var category = _config.LedgerAccountsList.FirstOrDefault(x => x.AccountType == ConfigModels.MoneyAccountType.NotSet && x.CategoryId == t.CategoryUID);
                    if (category is null)
                    {
                        // Possible that it's an untracked adjustment
                        // I'll leave the exception here for now but I may have to replace it with the other Special Account
                        throw new MoneyRecordConversionException(t, "Category account not found");
                    }
                    else if (category is PayableAccount payable)
                    {
                        newRecord.DebitAccount = payable;
                        if (money is BankAccount bank)
                        {
                            newRecord.CreditAccount = bank;
                        }
                        else if (money is CreditCardAccount card)
                        {
                            newRecord.CreditAccount = card;
                        }
                        else
                        {
                            throw new MoneyRecordConversionException(t, string.Format("Money Account Type [{0}] not supported for this category", money.GetType().FullName));
                        }
                    }
                    else if (category is ReceivableAccount receivable)
                    {
                        newRecord.CreditAccount = receivable;
                        if (money is BankAccount bank)
                        {
                            newRecord.DebitAccount = bank;
                        }
                        else
                        {
                            throw new MoneyRecordConversionException(t, string.Format("Money Account Type [{0}] not supported for this category", money.GetType().FullName));
                        }
                    }
                    else
                    {
                        throw new MoneyRecordConversionException(t, string.Format("Category [{0}] not supported for regular entry", category.GetType().FullName));
                    }

                }



                _listTransactions.Add(newRecord);
                ledger.RemoveTransaction(t);
                newRecord = null;
            }

            this.SaveToFile();
            ledger.SaveToFile();
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }

    public class MoneyRecordConversionException : Exception
    {
        private readonly IMoneyRecord _original;
        public IMoneyRecord OriginalRecord { get { return _original; } }

        public MoneyRecordConversionException(IMoneyRecord legacy) : base()
        {
            _original = legacy;
        }
        public MoneyRecordConversionException(IMoneyRecord legacy, string message) : base(message)
        {
            _original = legacy;
        }
        public MoneyRecordConversionException(IMoneyRecord legacy, string message, Exception inner) : base(message, inner)
        {
            _original = legacy;
        }


        public override string Message
        {
            get
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("CONVERSION FAILED: {0}", base.Message).AppendLine();
                msg.AppendFormat("LEGACY RECORD: {0}", JsonSerializer.Serialize(_original)).AppendLine();

                if (base.InnerException != null)
                {
                    msg.AppendLine(base.InnerException.ToString());
                }

                return msg.ToString();
            }
        }
    }


}