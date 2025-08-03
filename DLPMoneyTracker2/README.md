
# Introduction

This WPF application is my personal finance tracking tool.  All data entered is stored in local JSON files OR a SQL Server. 

### Why not use existing apps like Quicken?

While the existing apps are quite robust, the versions I tried would not work without linking accounts.  While this may be nice for some people to better facilitate Bank Reconciliation, for me it often led to duplicate entries as my manual entries wouldn't automatically match up to an existing bank record.  This caused too much micromanagement with data fixes which left reporting untrustworthy.


# Configuration

These are the objects that should be defined before using this application.

### Money Accounts

Money accounts are all Real accounts such as Bank accounts, Credit Cards, and Loans.

### Ledger Accounts

Ledger accounts are all Nominal accounts used for reporting purposes.  

### Budget Plans

Recurring income and expenses be they annual, monthly, etc.


# Features

These are the features available to the user.

### Account Summary

Quick overview of **Money Accounts** including current balances and upcoming budget plans.


### Budget Planner

Compares all Income Budget Plans against all Expense Budget Plans plus variable expenses with an option to assign an arbitrary budget amount to said variable expenses.
Intended to show expenses vs income to evaluate whether I'm living within my means.

**NOTE: Needs some rethinking as it's not working as intended.**


### Bill Reminders

Uses Expense Budget Plans to determine which recurring expenses have been paid and which are coming up.

**NOTE: there are some holes in the monthly dates.  Likely due to bills being paid early.  May need some reworking.**


### Year to Date

Shows summations of each Nominal account by Month and Year to Date totals.  Solely FYI in case I'm every curious where all my money is going.


### Bank Reconciliation

As with any bank reconciliation software, this is intended to vette the accuracy of transactions by comparing the sum of transaction within certain bank date ranges to the totals listed on offical bank statements.  

**NOTE: This need some work.  It was designed back when this app only operated on a yearly basis and required resetting at the end of each year.  Needs to be rethought to show past 12 months.**

### Transaction Entry

All transactions contain the following details:
- Transaction Date
- Description
- Debit Account / Credit Account
- Amount
- Bank Dates (where applicable)

These transactions are intended to be as simple as possible with only one Debit Account and only one Credit Account per transaction.  My life is not so complicated that I need to categorize each item on my receipts.  Generally, depending on the transaction type, one account will be a **"Money"** account and the other will be a **"Category"** account.  

### Import CSV

This feature was added to offer a quick way to take a CSV listing of money account activity and either match it to existing transactions, thereby applying the bank date to the transaction, OR to create a new transaction from the record.  Should be done once a quarter if not monthly to assert accuracy of transactions.

### Import / Export JSON

This option is used to sync the local JSON files with a SQL database.  At some point down the road, I may automate this.







