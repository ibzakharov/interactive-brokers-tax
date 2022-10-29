using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Investing.Common.Models;
using Investing.Common.Stores;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Investing.Common.Constants;

namespace Investing.Common.Services
{
    public class ExcelDividendSheetService
    {
        private readonly ExcelSheetService _excelSheetService;
        private readonly DividendStore _dividendStore;
        private readonly DividendTaxStore _dividendTaxStore;
        private readonly TitleProvider _titleProvider;
        private readonly ExcelRangeService _excelRangeService;

        public ExcelDividendSheetService(ExcelSheetService excelSheetService, 
            DividendStore dividendStore,
            DividendTaxStore dividendTaxStore,
            TitleProvider titleProvider,
            ExcelRangeService excelRangeService)
        {
            _excelSheetService = excelSheetService;
            _dividendStore = dividendStore;
            _dividendTaxStore = dividendTaxStore;
            _titleProvider = titleProvider;
            _excelRangeService = excelRangeService;
        }

        private ExcelWorksheet sheet => _excelSheetService.GetDividendsSheet;

        public void Generate(int year)
        {
            var dividends = _dividendStore.ByYear(year);

            if (dividends.Count == 0)
                return;
            
            var row = 1;
            row++;

            var title = $"Доходы по дивидендам за период 01.01.{year} - 31.12.{year}";
            _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Eleventh,
                title, Colors.MainTitleColor);
            
            row++;
            row++;

            var groupBySymbol = dividends.GroupBy(d => d.Symbol);

            var totalRows = new List<int>();

            foreach (var group in groupBySymbol)
            {
                var dividendsBySymbol = group.ToList();

                var symbolTitle = _titleProvider.GetTitleName(group.Key, dividendsBySymbol.First().Currency);
                CreateSymbolTitleHeader(row, symbolTitle);
                row++;
                CreateIncomeTitleHeader(row);
                CreateForeignTaxTitleHeader(row);
                CreateRussianTaxTitleHeader(row);
                row++;
                CreateIncomeHeader(row);
                CreateTaxHeader(row);
                CreateForeignTaxHeader(row);
                CreateRussianTaxHeader(row);
                row++;

                foreach (var dividend in dividendsBySymbol)
                {
                    WriteDividend(ref row, dividend);
                    var dividendTax = _dividendTaxStore.ByDateAndSymbol(dividend.Date, dividend.Symbol);

                    if (dividendTax == null)
                    {
                        dividendTax = new DividendTax
                        {
                            Currency = dividend.Currency,
                            Date = dividend.Date,
                            Sum = 0,
                            Symbol = dividend.Symbol
                        };
                    }

                    WriteDividendTaxPaid(dividendTax, row);
                    WriteDividendTaxOutcome(row);

                    totalRows.Add(row);

                    row++;
                }

                row++;
            }

            WriteTotal(row, totalRows);
        }

        private void CreateSymbolTitleHeader(int row, string title)
        {
            var range = sheet.Cells[row, 1, row, 11];
            range.Merge = true;
            range.Value = title;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            range.Style.Font.Bold = true;
            _excelRangeService.SetBorder(range);
        }
        
        private void CreateIncomeTitleHeader(int row)
        {
            var range = sheet.Cells[row, 1, row, 5];
            range.Merge = true;
            range.Value = "Выручка";
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.IncomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            _excelRangeService.SetBorder(range);
        }

        private void CreateForeignTaxTitleHeader(int row)
        {
            var range = sheet.Cells[row, 7, row, 9];
            range.Merge = true;
            range.Value = "Налог удержан";
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.OutcomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            _excelRangeService.SetBorder(range);
        }

        private void CreateRussianTaxTitleHeader(int row)
        {
            var range = sheet.Cells[row, 10, row, 11];
            range.Merge = true;
            range.Value = "Налог к уплате в РФ";
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.OutcomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            range.AutoFitColumns();
            _excelRangeService.SetBorder(range);
        }

        private void CreateIncomeHeader(int row)
        {
            var color = Colors.IncomeColor;
            InitializeTradeHeaderCell(row, ExcelColumnNumber.First, "Дата", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Second, "В валюте", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Third, "Курс", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Fourth, "В рублях", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Fifth, "Код дохода", color, true);
        }

        private void CreateTaxHeader(int row)
        {
            var range = sheet.Cells[row - 1, ExcelColumnNumber.Sixth, row, ExcelColumnNumber.Sixth];
            range.Merge = true;
            range.Value = "Сумма налога в рублях";
            range.Style.WrapText = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.OutcomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            range.AutoFitColumns(15);
            _excelRangeService.SetBorder(range);
        }

        private void CreateForeignTaxHeader(int row)
        {
            var color = Colors.OutcomeColor;
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Seventh, "Ставка (%)", color, true);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Eighth, "В валюте", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Ninth, "В рублях", color);
        }

        private void CreateRussianTaxHeader(int row)
        {
            var color = Colors.OutcomeColor;
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Tenth, "Ставка (%)", color, true);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Eleventh, "В рублях", color);
        }

        private void InitializeTradeHeaderCell(int row, int column, string value,
            Color color)
        {
            var range = sheet.Cells[row, column];
            range.Value = value;

            if (color != Color.White)
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(color);
            }

            _excelRangeService.SetBorder(range);
        }

        private void InitializeTradeHeaderCell(int row, int column, string value,
            CellColor color, bool autoFit = false)
        {
            var range = sheet.Cells[row, column];
            range.Value = value;

            if (color != null)
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            }

            if (autoFit) range.AutoFitColumns();
            
            _excelRangeService.SetBorder(range);
        }

        private void WriteDividend(ref int row, Dividend dividend)
        {
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeCell = sheet.Cells[row, ExcelColumnNumber.First];
            dateTimeCell.Style.Numberformat.Format = dateTimeFormat;
            dateTimeCell.Value = dividend.Date.ToString(dateTimeFormat);
            dateTimeCell.AutoFitColumns();

            var sumCell = sheet.Cells[row, ExcelColumnNumber.Second];
            sumCell.Value = dividend.Sum;
            sumCell.Style.Numberformat.Format = "0.00";

            var rate = ExchangeRateProvider.Get(dividend.Currency, dividend.Date);
            var rateCell = sheet.Cells[row, ExcelColumnNumber.Third];
            rateCell.Value = rate.Value;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Fourth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{ExcelColumnName.Second}{row}*{ExcelColumnName.Third}{row}";

            var codeCell = sheet.Cells[row, ExcelColumnNumber.Fifth];
            codeCell.Value = 1010;

            var taxRubCell = sheet.Cells[row, ExcelColumnNumber.Sixth];
            taxRubCell.Style.Numberformat.Format = "0.00";
            taxRubCell.Formula = $"{ExcelColumnName.Fourth}{row}*13%";

            var taxRateCell = sheet.Cells[row, ExcelColumnNumber.Tenth];
            taxRateCell.Style.Numberformat.Format = "0";
            taxRateCell.Formula =
                $"100*{ExcelColumnName.Eleventh}{row}/{ExcelColumnName.Fourth}{row}";
        }

        private void WriteDividendTaxPaid(DividendTax dividendTax, int row)
        {
            var taxRateCell = sheet.Cells[row, ExcelColumnNumber.Seventh];
            taxRateCell.Style.Numberformat.Format = "0";
//=IF(100*K90/D90<0;0;100*K90/D90)
            taxRateCell.Formula =
                $"100*{ExcelColumnName.Ninth}{row}/{ExcelColumnName.Fourth}{row}";
                //$"=IF(100*{ExcelColumnName.Ninth}{row}/{ExcelColumnName.Fourth}{row}<0;0;100*{ExcelColumnName.Ninth}{row}/{ExcelColumnName.Fourth}{row})";
            
            var sumCell = sheet.Cells[row, ExcelColumnNumber.Eighth];
            sumCell.Value = dividendTax.SumAbs;
            sumCell.Style.Numberformat.Format = "0.00";

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Ninth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{ExcelColumnName.Third}{row}*{ExcelColumnName.Eight}{row}";
        }

        private void WriteDividendTaxOutcome(int row)
        {
            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Eleventh];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{ExcelColumnName.Sixth}{row}-{ExcelColumnName.Ninth}{row}";
        }

        private void WriteTotal(int row, List<int> totalRows)
        {
            var totalCell = sheet.Cells[row, 1, row, 1];
            totalCell.Merge = true;
            totalCell.Value = "ИТОГО";

            var sum = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Second}{r}"));
            var sumRub = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Fourth}{r}"));
            var sumTotalTax = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Sixth}{r}"));
            var sumPaidTax = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Eight}{r}"));
            var sumPaidRubTax = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Ninth}{r}"));
            var sumTax = string.Join("+", totalRows.Select(r => $"{ExcelColumnName.Eleventh}{r}"));
            
            var sumCell = sheet.Cells[row, ExcelColumnNumber.Second];
            sumCell.Style.Numberformat.Format = "0.00";
            sumCell.Formula = $"{sum}";
            _excelRangeService.SetBorder(sumCell);         
            
            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Fourth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{sumRub}";
            _excelRangeService.SetBorder(sumRubCell);

            // var emptyCell = sheet.Cells[row, 6];
            // emptyCell.Merge = true;
            // SetBorder(emptyCell);

            var sumTotalTaxCell = sheet.Cells[row, ExcelColumnNumber.Sixth];
            sumTotalTaxCell.Style.Numberformat.Format = "0.00";
            sumTotalTaxCell.Formula = $"{sumTotalTax}";
            _excelRangeService.SetBorder(sumTotalTaxCell);

            // emptyCell = sheet.Cells[row, 8];
            // emptyCell.Merge = true;
            // SetBorder(emptyCell);

            var sumPaidTaxCell = sheet.Cells[row, ExcelColumnNumber.Eighth];
            sumPaidTaxCell.Style.Numberformat.Format = "0.00";
            sumPaidTaxCell.Formula = $"{sumPaidTax}";
            _excelRangeService.SetBorder(sumPaidTaxCell);    
            
            var sumPaidRubTaxCell = sheet.Cells[row, ExcelColumnNumber.Ninth];
            sumPaidRubTaxCell.Style.Numberformat.Format = "0.00";
            sumPaidRubTaxCell.Formula = $"{sumPaidRubTax}";
            _excelRangeService.SetBorder(sumPaidRubTaxCell);

            // emptyCell = sheet.Cells[row, 10];
            // emptyCell.Merge = true;
            // SetBorder(emptyCell);

            var sumTaxCell = sheet.Cells[row, ExcelColumnNumber.Eleventh];
            sumTaxCell.Style.Numberformat.Format = "0.00";
            sumTaxCell.Formula = $"{sumTax}";
            _excelRangeService.SetBorder(sumTaxCell);

            var allCells = sheet.Cells[row, 1, row, 11];
            allCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            allCells.Style.Fill.BackgroundColor.SetColor(Colors.TotalColor);
            allCells.Style.Font.Bold = true;
        }
    }
}