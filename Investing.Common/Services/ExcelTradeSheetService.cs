using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Investing.Common.Models;
using Investing.Resources;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Investing.Common.Constants;

namespace Investing.Common.Services
{
    public class ExcelTradeSheetService
    {
        private readonly ExcelSheetService _excelSheetService;
        private readonly TitleProvider _titleProvider;
        private readonly ExcelRangeService _excelRangeService;

        public ExcelTradeSheetService(ExcelSheetService excelSheetService,
            TitleProvider titleProvider,
            ExcelRangeService excelRangeService)
        {
            _excelSheetService = excelSheetService;
            _titleProvider = titleProvider;
            _excelRangeService = excelRangeService;
        }

        private ExcelWorksheet sheet => _excelSheetService.GetTradesSheet;

        public void Generate(int year, List<Trade> list, Config config)
        {
            var row = 1;

            var securities = list.Where(i => i.ActiveType == "Акции").ToList();
            var warrants = list.Where(i => i.ActiveType == "Варранты").ToList();

            if (securities.Count == 0 && warrants.Count == 0)
                return;
            
            var totalTradeCount = securities.Count(t => t.Operation == Operation.Close);
            decimal commissionByPosition = 0;
            if (config.HasCommission)
            {
                commissionByPosition = config.Commission / totalTradeCount;
            }

            row++;
            var title =
                $"Доходы от операций с ценными бумагами и производными финансовыми инструментами за период 01.01.{year} - 31.12.{year}";
            _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Fourteenth,
                title, Colors.MainTitleColor);
            row++;
            row++;

            var totalRows = new List<int>();
            var subTotalRows = new List<int>();

            if (warrants.Count > 0)
            {
                GenerateWarrants(warrants, ref row, totalRows, commissionByPosition, subTotalRows);
            }

            if (securities.Count > 0)
            {
                GenerateSecurities(securities, ref row, totalRows, commissionByPosition, subTotalRows);
            }

            WriteTotal(row, subTotalRows);

            for (int i = 1; i < 20; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }

        private void GenerateSecurities(List<Trade> list, ref int row, List<int> totalRows, 
            decimal commissionByPosition, List<int> subTotalRows)
        {
            _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Fourteenth,
                "Акции", Colors.SecurityTitleColor);
            row++;
            row++;

            var groupCurrencies = list.OrderBy(i => i.Currency).GroupBy(i => i.Currency);
            
            foreach (var groupCurrency in groupCurrencies)
            {
                var tradesByCurrency = groupCurrency.ToList();
                foreach (var groupSymbol in tradesByCurrency.OrderBy(i => i.Symbol).GroupBy(i => i.Symbol))
                {
                    var queue = UnionTrades(groupSymbol.ToList());

                    var symbolTitle = _titleProvider.GetTitleName(groupSymbol.Key, groupCurrency.Key);
                    _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Fourteenth,
                        symbolTitle, Colors.SymbolTitleColor);
                    
                    var totalRows1 = new List<int>();

                    row++;
                    CreateIncomeTitleHeader(row, tradesByCurrency[0]);
                    CreateOutcomeTitleHeader(row, tradesByCurrency[0]);
                    row++;
                    CreateIncomeHeader(row);
                    CreateOutcomeHeader(row);

                    foreach (var trades in queue)
                    {
                        row++;
                        var startRow = row;
                        WriteTrade(ref row, trades, commissionByPosition);
                        var endRow = row;
                        row++;
                        WriteTotalTradeIncome(row, startRow, endRow);
                        WriteTotalTradeOutcome(row, startRow, endRow);
                        subTotalRows.Add(row);
                        totalRows1.Add(row);
                    }

                    row++;

                    WriteTotalTrade(row, totalRows1);
                    totalRows.Add(row);

                    row++;
                    row++;
                }
            }
        }

        private void GenerateWarrants(List<Trade> list, ref int row, List<int> totalRows,
            decimal commissionByPosition, List<int> subTotalRows)
        {
            _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Fourteenth,
                "Варранты", Colors.SecurityTitleColor);
            row++;
            row++;
            
            foreach (var groupCurrency in list.OrderBy(i => i.Currency).GroupBy(i => i.Currency))
            {
                var tradesByCurrency = groupCurrency.ToList();
                foreach (var groupSymbol in tradesByCurrency.OrderBy(i => i.Symbol).GroupBy(i => i.Symbol))
                {
                    var symbolTitle = _titleProvider.GetTitleName(groupSymbol.Key, groupCurrency.Key);
                    _excelRangeService.CreateTitleHeader(sheet, row, ExcelColumnNumber.First, ExcelColumnNumber.Fourteenth,
                        symbolTitle, Colors.SymbolTitleColor);

                    var totalRows1 = new List<int>();
                    
                    row++;
                    CreateIncomeTitleHeader(row, tradesByCurrency[0]);
                    CreateOutcomeTitleHeader(row, tradesByCurrency[0]);
                    row++;
                    CreateIncomeHeader(row);
                    CreateOutcomeHeader(row);

                    row++;
                    var startRow = row;

                    var tr = groupSymbol.ToList();
                    WriteTrade(ref row, tr, commissionByPosition);
                    var endRow = row;
                    row++;
                    WriteTotalTradeIncome(row, startRow, endRow);
                    WriteTotalTradeOutcome(row, startRow, endRow);
                    subTotalRows.Add(row);
                    totalRows1.Add(row);
                    row++;
                    WriteTotalTrade(row, totalRows1);
                    totalRows.Add(row);
                    
                    row++;
                    row++;
                }
            }
        }

        private void WriteTotal(int row, List<int> subTotalRows)
        {
            var totalCell = sheet.Cells[row, ExcelColumnNumber.First, row, ExcelColumnNumber.Sixth];
            totalCell.Merge = true;
            totalCell.Value = "Общая сумма доходов";
            totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.IncomeColor;
            totalCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            totalCell.Style.Font.Bold = true;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Seventh];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula =
                $"{string.Join("+", subTotalRows.Select(i => $"{ExcelColumnName.Seventh}{i}"))}";
            sumRubCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            sumRubCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            sumRubCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(sumRubCell);

            row++;
            
            totalCell = sheet.Cells[row, ExcelColumnNumber.First, row, ExcelColumnNumber.Sixth];
            totalCell.Merge = true;
            totalCell.Value = "Общая сумма расходов";
            totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            color = Colors.OutcomeColor;
            totalCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            totalCell.Style.Font.Bold = true;

            sumRubCell = sheet.Cells[row, ExcelColumnNumber.Seventh];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula =
                $"{string.Join("+", subTotalRows.Select(i => $"{ExcelColumnName.Fourteenth}{i}"))}";
            sumRubCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            sumRubCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            sumRubCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(sumRubCell);

            row++;
            
            totalCell = sheet.Cells[row, ExcelColumnNumber.First, row, ExcelColumnNumber.Sixth];
            totalCell.Merge = true;
            totalCell.Value = "Итого прибыль/убыток";
            totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totalCell.Style.Fill.BackgroundColor.SetColor(Colors.TotalColor);
            totalCell.Style.Font.Bold = true;

            sumRubCell = sheet.Cells[row, ExcelColumnNumber.Seventh];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{ExcelColumnName.Seventh}{row-2}-{ExcelColumnName.Seventh}{row-1}";
            sumRubCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            sumRubCell.Style.Fill.BackgroundColor.SetColor(Colors.TotalColor);
            sumRubCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(sumRubCell);
        }
        
        private void WriteTotalTrade(int row, List<int> totalRows)
        {
            var listProfit = new List<string>();
            var listLoss = new List<string>();
            foreach (var totalRow in totalRows)
            {
                listProfit.Add($"{ExcelColumnName.Seventh}{totalRow}");
                listLoss.Add($"{ExcelColumnName.Fourteenth}{totalRow}");
            }

            var sumProfit = string.Join("+", listProfit);
            var sumLoss = string.Join("+", listLoss);
            var formula = $"{sumProfit}-({sumLoss})";
            
            var totalCell = sheet.Cells[row, 1, row, 6];
            totalCell.Merge = true;
            totalCell.Value = "Итого прибыль/убыток по позиции";
            _excelRangeService.SetBorder(totalCell);
            
            var sumRubCell = sheet.Cells[row, 7];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = formula;
            _excelRangeService.SetBorder(sumRubCell);

            var positiveFormula = $"{ExcelColumnName.Seventh}{row}>=0";
            AddConditionalFormatting(sumRubCell, positiveFormula, Colors.PositiveProfitLossColor);
            AddConditionalFormatting(totalCell, positiveFormula, Colors.PositiveProfitLossColor);
            
            var negativeFormula = $"{ExcelColumnName.Seventh}{row}<0";
            AddConditionalFormatting(sumRubCell,negativeFormula, Colors.NegativeProfitLossColor);
            AddConditionalFormatting(totalCell, negativeFormula, Colors.NegativeProfitLossColor);
        }

        private void AddConditionalFormatting(ExcelRange range, string formula, CellColor color)
        {
            var lessThan = range.ConditionalFormatting.AddExpression();
            lessThan.Style.Fill.PatternType = ExcelFillStyle.Solid;
            lessThan.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(color.Red, color.Green, color.Blue));
            lessThan.Style.Font.Bold = true;
            lessThan.Formula = formula;
        }

        private void WriteTrade(ref int row, List<Trade> trades, decimal commission)
        {
            var rowIncome = row;
            var rowOutcome = row;

            foreach (var trade in trades)
            {
                if (trade.Operation == Operation.Close)
                {
                    WriteTradeIncome(trade, rowIncome);
                    rowIncome++;
                }
                else
                {
                    WriteTradeOutcome(trade, rowOutcome);
                    rowOutcome++;
                }

                WriteTradeCommission(trade, rowOutcome);
                rowOutcome++;
            }

            if (commission > 0)
            {
                WriteCommission(rowOutcome, commission);
                rowOutcome++;
            }

            row = rowOutcome - 1;
        }

        private void WriteTradeOutcome(Trade trade, int row)
        {
            // Date
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeCell = sheet.Cells[row, ExcelColumnNumber.Eighth];
            dateTimeCell.Style.Numberformat.Format = dateTimeFormat;
            dateTimeCell.Value = trade.DateTime.ToString(dateTimeFormat);
            dateTimeCell.AutoFitColumns();

            // Quantity
            var quantityCell = sheet.Cells[row, ExcelColumnNumber.Tenth];
            quantityCell.Value = trade.QuantityAbs;

            // Price
            var priceCell = sheet.Cells[row, ExcelColumnNumber.Ninth];
            priceCell.Style.Numberformat.Format = "0.0000";
            priceCell.Value = trade.TransactionPrice;

            // Sum
            var sumCell = sheet.Cells[row, ExcelColumnNumber.Eleventh];
            sumCell.Formula =
                $"{ExcelColumnName.Tenth}{row}*{ExcelColumnName.Ninth}{row}";
            sumCell.Style.Numberformat.Format = "0.00";

            var rate = ExchangeRateProvider.Get(trade.Currency, trade.DateTime.Date);
            var rateCell = sheet.Cells[row, ExcelColumnNumber.Twelfth];
            rateCell.Value = rate.Value;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Thirteenth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula =
                $"{ExcelColumnName.Eleventh}{row}*{ExcelColumnName.Twelfth}{row}";

            var typeCell = sheet.Cells[row, ExcelColumnNumber.Fourteenth];
            typeCell.Value = "Покупка акций";
        }

        private void WriteTradeIncome(Trade trade, int row)
        {
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeCell = sheet.Cells[row, ExcelColumnNumber.First];
            dateTimeCell.Style.Numberformat.Format = dateTimeFormat;
            dateTimeCell.Value = trade.DateTime.ToString(dateTimeFormat);
            dateTimeCell.AutoFitColumns();

            var quantityCell = sheet.Cells[row, ExcelColumnNumber.Third];
            quantityCell.Value = trade.QuantityAbs;

            var priceCell = sheet.Cells[row, ExcelColumnNumber.Second];
            priceCell.Style.Numberformat.Format = "0.0000";
            priceCell.Value = trade.TransactionPrice;

            var sumCell = sheet.Cells[row, ExcelColumnNumber.Fourth];
            sumCell.Formula =
                $"{ExcelColumnName.Third}{row}*{ExcelColumnName.Second}{row}";
            sumCell.Style.Numberformat.Format = "0.00";

            var rate = ExchangeRateProvider.Get(trade.Currency, trade.DateTime.Date);
            var rateCell = sheet.Cells[row, ExcelColumnNumber.Fifth];
            rateCell.Value = rate.Value;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Sixth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula =
                $"{ExcelColumnName.Fourth}{row}*{ExcelColumnName.Fifth}{row}";

            var typeCell = sheet.Cells[row, ExcelColumnNumber.Seventh];
            typeCell.Value = "Продажа акций";
        }

        private void WriteTradeCommission(Trade trade, int row)
        {
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeCell = sheet.Cells[row, ExcelColumnNumber.Eighth];
            dateTimeCell.Style.Numberformat.Format = dateTimeFormat;
            dateTimeCell.Value = trade.DateTime.ToString(dateTimeFormat);
            dateTimeCell.AutoFitColumns();

            var quantityCell = sheet.Cells[row, ExcelColumnNumber.Tenth];
            quantityCell.Value = trade.QuantityAbs;

            var sumCell = sheet.Cells[row, ExcelColumnNumber.Eleventh];
            sumCell.Value = trade.ComissionAbs;
            sumCell.Style.Numberformat.Format = "0.00";

            var rate = ExchangeRateProvider.Get(trade.Currency, trade.DateTime.Date);
            var rateCell = sheet.Cells[row, ExcelColumnNumber.Twelfth];
            rateCell.Value = rate.Value;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Thirteenth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula =
                $"{ExcelColumnName.Eleventh}{row}*{ExcelColumnName.Twelfth}{row}";

            var typeCell = sheet.Cells[row, ExcelColumnNumber.Fourteenth];
            typeCell.Value = trade.Operation == Operation.Close
                ? "Комиссия за продажу акций"
                : "Комиссия за покупку акций";
        }
        
        private void WriteCommission(int row, decimal commission)
        {
            var rateCell = sheet.Cells[row, ExcelColumnNumber.Twelfth];
            rateCell.Value = 1;

            var sumRubCell = sheet.Cells[row, ExcelColumnNumber.Thirteenth];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Value = commission;

            var typeCell = sheet.Cells[row, ExcelColumnNumber.Fourteenth];
            typeCell.Value = "Общие расходы";
        }

        private void WriteTotalTradeIncome(int row, int startRow, int endRow)
        {
            var list = new List<string>();
            for (int i = startRow; i <= endRow; i++)
            {
                list.Add($"{ExcelColumnName.Sixth}{i}");
            }

            var color = Colors.IncomeColor;
            
            var totalCell = sheet.Cells[row, 1, row, 6];
            totalCell.Merge = true;
            totalCell.Value = "Итого доход по операции";
            totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totalCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            totalCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(totalCell);

            var sumRubCell = sheet.Cells[row, 7];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{string.Join("+", list)}";
            sumRubCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            sumRubCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            sumRubCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(sumRubCell);
        }

        private void WriteTotalTradeOutcome(int row, int startRow, int endRow)
        {
            var list = new List<string>();
            for (int i = startRow; i <= endRow; i++)
            {
                list.Add($"{ExcelColumnName.Thirteenth}{i}");
            }

            var color = Colors.OutcomeColor;
            
            var totalCell = sheet.Cells[row, 8, row, 13];
            totalCell.Merge = true;
            totalCell.Value = "Итого расход по операции";
            totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totalCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            totalCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(totalCell);

            var sumRubCell = sheet.Cells[row, 14];
            sumRubCell.Style.Numberformat.Format = "0.00";
            sumRubCell.Formula = $"{string.Join("+", list)}";
            sumRubCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            sumRubCell.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            sumRubCell.Style.Font.Bold = true;
            _excelRangeService.SetBorder(sumRubCell);
        }

        private void CreateIncomeTitleHeader(int row, Trade trade)
        {
            var code = GetIncomeCode(trade);

            var range = sheet.Cells[row, 1, row, 7];
            range.Merge = true;
            range.Value = $"Выручка    Код: {code}";
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.IncomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            _excelRangeService.SetBorder(range);
        }

        private static int GetIncomeCode(Trade trade)
        {
            int code = 0;

            if (trade.ActiveType == "Акции")
            {
                code = 1530;
            }
            else if (trade.ActiveType == "Варранты")
            {
                code = 1532;
            }

            return code;
        }      
        
        private static int GetOutcomeCode(Trade trade)
        {
            int code = 0;

            if (trade.ActiveType == "Акции")
            {
                code = 201;
            }
            else if (trade.ActiveType == "Варранты")
            {
                code = 206;
            }

            return code;
        }

        private void CreateOutcomeTitleHeader(int row, Trade trade)
        {
            var code = GetOutcomeCode(trade);
            
            var range = sheet.Cells[row, 8, row, 14];
            range.Merge = true;
            range.Value = $"Расход    Код: {code}";
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            var color = Colors.OutcomeColor;
            range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            _excelRangeService.SetBorder(range);
        }

        private void CreateIncomeHeader(int row)
        {
            var color = Colors.IncomeColor;
            InitializeTradeHeaderCell(row, ExcelColumnNumber.First, "Дата", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Second, "Цена", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Third, "Кол-во",
                color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Fourth, "В валюте", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Fifth, "Курс", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Sixth, "В рублях", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Seventh, "Вид дохода", color);
        }

        private void CreateOutcomeHeader(int row)
        {
            var color = Colors.OutcomeColor;
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Eighth, "Дата", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Ninth, "Цена", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Tenth, "Кол-во",
                color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Eleventh, "В валюте", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Twelfth, "Курс", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Thirteenth, "В рублях", color);
            InitializeTradeHeaderCell(row, ExcelColumnNumber.Fourteenth, "Вид расхода",
                color);
        }

        private void InitializeTradeHeaderCell(int row, int column, string value, CellColor color)
        {
            var range = sheet.Cells[row, column];
            range.Value = value;

            if (color != null)
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(color.Alpha, color.Red, color.Green, color.Blue);
            }

            _excelRangeService.SetBorder(range);
        }

        private List<List<Trade>> UnionTrades(List<Trade> trades)
        {
            var queue = new List<List<Trade>>();

            List<Trade> list = new List<Trade>();
            foreach (var trade in trades)
            {
                if (trade.Operation == Operation.Open)
                {
                    list.Add(trade);
                }
                else
                {
                    if (list.Count == 0)
                        throw new TradeUnionException("Ошибка при объединении сделок по символу " + trades[0].Symbol);
                    list.Add(trade);
                    queue.Add(list);
                    list = new List<Trade>();
                }
            }

            return queue;
        }
    }
}