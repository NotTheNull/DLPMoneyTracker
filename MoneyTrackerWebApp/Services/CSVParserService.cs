using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using Microsoft.AspNetCore.Components.Forms;
using MoneyTrackerWebApp.Models.CSVImport;

namespace MoneyTrackerWebApp.Services
{
    public interface ICSVParserService
    {
        Task<CSVFileDTO> GetDataFromCSV(IMoneyAccount account, IBrowserFile file);
    }

    public class CSVParserService : ICSVParserService
    {

        public async Task<CSVFileDTO> GetDataFromCSV(IMoneyAccount account, IBrowserFile file)
        {
            if (account?.Mapping?.IsValidMapping() != true) return null;

            string text = await GetFileText(file);
            List<string[]> records = ParseFileText(text);
            return BuildDTO(account, records);
        }


        private CSVFileDTO BuildDTO(IMoneyAccount account, List<string[]> rowData)
        {
            CSVFileDTO dto = new CSVFileDTO();
            if (rowData?.Any() != true) return dto;

            for (int i = account.Mapping.StartingRow - 1; i < rowData.Count; i++)
            {
                if (rowData[i].Length <= 1)
                {
                    // Row did not have any commas, must be a blank row; skip it
                    continue;
                }

                dto.RecordList.Add(new CSVRecord
                {
                    TransactionDate = rowData[i][account.Mapping.GetMapping(ICSVMapping.TRANS_DATE)].RemoveQuotes().ToDateTime(),
                    Description = rowData[i][account.Mapping.GetMapping(ICSVMapping.DESCRIPTION)].RemoveQuotes().Trim(),
                    Amount = rowData[i][account.Mapping.GetMapping(ICSVMapping.AMOUNT)].RemoveQuotes().ToDecimal()
                });
            }

            return dto;
        }


        private async Task<string> GetFileText(IBrowserFile file)
        {

            string text = "";
            using (var stream = file.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    text = await reader.ReadToEndAsync();
                }
            }

            return text;

        }


        private List<string[]> ParseFileText(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return null;

            string splitThis = "\n";
            if (csv.Contains("\n\r"))
            {
                splitThis = "\n\r";
            }
            else if (csv.Contains("\r\n"))
            {
                splitThis = "\r\n";
            }

            string[] csvRecords = csv.Split(splitThis);
            List<string[]> splitRecords = new List<string[]>();

            foreach (string record in csvRecords)
            {
                string line = this.SanitizeCSVLine(record);
                splitRecords.Add(line.Split(","));
            }
            return splitRecords;
        }




        /// <summary>
        /// Removes any commas that are part of any descriptions or thousands separator so that
        /// it does not cause issues when splitting the CSV rows
        ///
        /// REMINDER: This data came from an outside source i.e. bank and, therefore, we have no control over it
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string SanitizeCSVLine(string arg)
        {
            char[] lineArray = arg.Trim().ToCharArray();
            bool isRemoveComma = false;
            for (int i = 0; i < lineArray.Length; i++)
            {
                if (lineArray[i] == '\"')
                {
                    isRemoveComma = !isRemoveComma;
                }
                else if (lineArray[i] == ',' && isRemoveComma)
                {
                    lineArray[i] = ' ';
                }
                // else continue
            }

            return new String(lineArray);
        }


    }
}
