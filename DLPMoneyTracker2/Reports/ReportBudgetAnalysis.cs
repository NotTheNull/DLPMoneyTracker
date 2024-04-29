using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.Core;
using FastReport;
using FastReport.Data;
using FastReport.ReportBuilder;
using FastReport.Utils;

namespace DLPMoneyTracker2.Reports
{
    public class ReportBudgetAnalysis
    {
        private readonly IGetBudgetAnalysisDataUseCase getAnalysisUseCase;

        public ReportBudgetAnalysis(IGetBudgetAnalysisDataUseCase getAnalysisUseCase)
        {
            this.getAnalysisUseCase = getAnalysisUseCase;
        }

        // NOTE: Fast Report uses SQL for Expressions
        public void Print(DateRange range, string title = "")
        {
            if (string.IsNullOrWhiteSpace(title)) title = string.Format("Budget Analysis: {0:yyyy/MM/dd} to {1:yyyy/MM/dd}", range.Begin, range.End);

            var listData = getAnalysisUseCase.Execute(range);

            System.Drawing.Font fontTitle = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold);
            System.Drawing.Font fontHeader = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            System.Drawing.Font fontDetail = new System.Drawing.Font("Times New Roman", 12);
            System.Drawing.RectangleF boundsDate = new System.Drawing.RectangleF(0, 0, 100, 30);
            System.Drawing.RectangleF boundsAccountName = new System.Drawing.RectangleF(0, 0, 180, 30);
            System.Drawing.RectangleF boundsDesc = new System.Drawing.RectangleF(0, 0, 320, 30);
            System.Drawing.RectangleF boundsAmount = new System.Drawing.RectangleF(0, 0, 200, 30);

            Report report = new Report();
            report.RegisterData(listData.OrderBy(o => o.IncomeOrExpense).ThenBy(o => o.TransactionDate), "DataList");
            report.GetDataSource("DataList").Enabled = true;

            ReportPage page = new ReportPage()
            {
                Name = "MainPage",
                TitleBeforeHeader = true
            };
            report.Pages.Add(page);

            page.ReportTitle = new ReportTitleBand()
            {
                Name = "ReportTitle",
                Height =  40.0f,
                Visible = true
            };
            TextObject reportTitle = new TextObject()
            {
                Name = "ReportTitleObject",
                Bounds = new System.Drawing.RectangleF(0, 0, 320, 50),
                Text = title,
                Font = fontTitle,
                HorzAlign = HorzAlign.Center
            };
            page.ReportTitle.Objects.Add(reportTitle);

            #region Page Header

            page.PageHeader = new PageHeaderBand()
            {
                Name = "PageHeaderLayout",
                Height = 60.0f,
                Visible = true
            };

            TextObject headerTransDate = new TextObject()
            {
                Name = "HeaderDate",
                Bounds = boundsDate,
                Text = "Transaction Date",
                Font = fontHeader,
                Border = new Border()
                {
                    BottomLine = new BorderLine()
                    {
                        Color = System.Drawing.Color.Black,
                        Style = LineStyle.Double,
                        Width = boundsDate.Width
                    }
                }
            };
            page.PageHeader.Objects.Add(headerTransDate);

            TextObject headerAccountName = new TextObject()
            {
                Name = "HeaderAccountName",
                Bounds = boundsAccountName,
                Text = "Account Name",
                Font = fontHeader,
                Border = new Border()
                {
                    BottomLine = new BorderLine()
                    {
                        Color = System.Drawing.Color.Black,
                        Style = LineStyle.Double,
                        Width = boundsAccountName.Width
                    }
                }
            };
            page.PageHeader.Objects.Add(headerAccountName);

            TextObject headerDescription = new TextObject()
            {
                Name = "HeaderDescription",
                Bounds = boundsDesc,
                Text = "Description",
                Font = fontHeader,
                Border = new Border()
                {
                    BottomLine = new BorderLine()
                    {
                        Color = System.Drawing.Color.Black,
                        Style = LineStyle.Double,
                        Width = boundsDesc.Width
                    }
                }
            };
            page.PageHeader.Objects.Add(headerDescription);


            TextObject headerTransAmount = new TextObject()
            {
                Name = "HeaderAmount",
                Bounds = boundsAmount,
                Text = "Amount",
                Font = fontHeader,
                HorzAlign = HorzAlign.Right,
                Border = new Border()
                {
                    BottomLine = new BorderLine()
                    {
                        Color = System.Drawing.Color.Black,
                        Style = LineStyle.Double,
                        Width = boundsAmount.Width
                    }
                }
            };
            page.PageHeader.Objects.Add(headerTransAmount);


            #endregion

            #region Group Header

            GroupHeaderBand group = new GroupHeaderBand()
            {
                Name = "GroupIncomeOrExpense",
                Height = 30.0f,
                Condition = "[DataList.IncomeOrExpense]",
                Visible = true
            };
            page.Bands.Add(group);

            TextObject groupHeaderTitle = new TextObject()
            {
                Name = "GroupTitleObject",
                Bounds = new System.Drawing.RectangleF(0, 0, 140, 40),
                Text = "[DataList.IncomeOrExpense]",
                Font = fontHeader
            };
            group.Objects.Add(groupHeaderTitle);



            #endregion

            #region Group Body

            DataBand groupData = new DataBand()
            {
                Name = "AnalysisData",
                Height = 30.0f,
                DataSource = report.GetDataSource("DataList")
            };
            group.Data = groupData;

            TextObject detailTransDate = new TextObject()
            {
                Name = "DetailDate",
                Bounds = boundsDate,
                Text = "[DataList.TransactionDate]",
                Format = new FastReport.Format.DateFormat() { Format = "yyyy/MM/dd" },
                Font = fontDetail,
                HorzAlign = HorzAlign.Center
            };
            groupData.Objects.Add(detailTransDate);

            TextObject detailAccountName = new TextObject()
            {
                Name = "DetailAccountName",
                Bounds = boundsAccountName,
                Text = "[DataList.AccountName]",
                Font = fontDetail
            };
            groupData.Objects.Add(detailAccountName);

            TextObject detailDescription = new TextObject()
            {
                Name = "DetailDescription",
                Bounds = boundsDesc,
                Text = "[DataList.TransactionDescription]",
                Font = fontDetail
            };
            groupData.Objects.Add(detailDescription);


            TextObject detailTransAmount = new TextObject()
            {
                Name = "DetailAmount",
                Bounds = boundsAmount,
                Text = "[DataList.TransactionAmount]",
                Format = new FastReport.Format.CurrencyFormat(),
                Font = fontDetail,
                HorzAlign = HorzAlign.Right
            };
            groupData.Objects.Add(detailTransAmount);

            #endregion

            #region Group Footer

            Total groupSum = new Total()
            {
                Name = "GroupSum",
                TotalType = TotalType.Sum,
                Evaluator = groupData,
                PrintOn = group.Footer
            };
            report.Dictionary.Totals.Add(groupSum);

            group.GroupFooter = new GroupFooterBand
            {
                Name = "GroupFooterIncomeOrExpense",
                Height = 30.0f,
                Visible = true
            };
            TextObject sumText = new TextObject()
            {
                Name = "GroupSumText",
                Bounds = new System.Drawing.RectangleF(0, 0, 200, 30),
                Text = "[GroupSum]",
                HorzAlign = HorzAlign.Right,
                Font = fontHeader
            };
            group.GroupFooter.Objects.Add(sumText);


            #endregion


            report.Prepare();
        }
    }
}
