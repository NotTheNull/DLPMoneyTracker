﻿namespace DLPMoneyTracker.Core.Models
{
    public interface ICSVMapping : IDisposable
    {
        public const string TRANS_DATE = "TransactionDate";
        public const string DESCRIPTION = "TransactionDescription";
        public const string AMOUNT = "TransactionAmount";

        int StartingRow { get; }
        bool IsAmountInverted { get; } // Normal: Negative amount = credit charge; Inverted: Negative amount = payment / income
        Dictionary<string, int> Mapping { get; }

        bool IsValidMapping();

        int GetMapping(string columnName);

        void SetMapping(string columnName, int columnIndex);

        void Copy(ICSVMapping cpy);
    }

    public class CSVMapping : ICSVMapping
    {
        public int StartingRow { get; set; } = 1;
        public bool IsAmountInverted { get; set; } = false;

        public Dictionary<string, int> Mapping { get; set; } = [];

        ~CSVMapping()
        {
            this.Dispose();
        }

        public bool IsValidMapping()
        {
            return this.StartingRow >= 0
                && this.Mapping.Count > 0;
        }

        public void Copy(ICSVMapping cpy)
        {
            if (cpy is null) return;

            this.StartingRow = cpy.StartingRow;
            this.IsAmountInverted = cpy.IsAmountInverted;
            this.Mapping.Clear();
            foreach (var item in cpy.Mapping)
            {
                this.Mapping.Add(item.Key, item.Value);
            }
        }

        public int GetMapping(string columnName)
        {
            // Remember to deduct 1 since arrays are 0-index
            if (this.Mapping.TryGetValue(columnName, out int value)) return value - 1;

            return -1;
        }

        public void SetMapping(string columnName, int columnIndex)
        {
            if (!Mapping.TryAdd(columnName, columnIndex))
            {
                this.Mapping[columnName] = columnIndex;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (this.Mapping != null)
            {
                this.Mapping.Clear();
            }
        }
    }
}