
# Introduction

This WPF application is my personal finance tracking tool.  All data entered is stored in local JSON files OR a SQL Server. 

### Why not use existing apps like Quicken?

While the existing apps are quite robust, the versions I tried would not work without linking accounts.  While this may be nice for some people to better facilitate Bank Reconciliation, for me it often led to duplicate entries as my manual entries wouldn't automatically match up to an existing bank record.  This caused too much micromanagement with data fixes which left reporting untrustworthy.


# AppSettings

There are two sections within the AppSettings.json file:
- **ConnectionStrings**
    - Feel free to add as many database connection strings as you need
	- Should add **"json_path"** as a connection string
	    - this is the path to where the data JSON files will be stored
		- If this is not defined, it will store the files in the same location as the application
- **AppSettings**
    - **"source"**
	    - defines which source-type (json or db) is the primary source of data
		- if not set, the UI will load Empty
	- **"connName"**
	    - the name of the database connection string to use
		- if not set, then all features requiring SQL _(e.g. Import/Export JSON)_ will not function
		- if you set the value to **"json_path"**, no data will load because it's not a valid SQL Server


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


# WIP

This section is intended to jot down ideas for future features

### Transaction Splitter

With some transactions (e.g. Amazon, Uber Eats, etc) I will usually record the transaction with the total but the bank will ultimately record multiple transactions _(e.g. Amazon purchases are split into multiple shipments, Uber Eats tip is charged separately from the Food order)_.
This feature is expected to create a new transaction prefilled with the transaction data, keep the same Debit / Credit accounts, and maintain valid Transaction amounts by re-evaluating the original transaction's amount.  The sum of the two records MUST total the original amount.

### Redesign Budget Planner

Mainly needs rethinking.  Right now, the math ends up not being accurate.  May want to replace the UI itself with a report be it SSRS or custom HTML.  

### Redesign Bank Reconciliation

The main thing with this existing feature is that it's not tailored for multiple years.  The UI alone needs to be reworked to represent previous 12 to 24 months.


