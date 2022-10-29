using System.Collections.Generic;
using System.Linq;
using Investing.Common.Models;
using Investing.Common.Services;
using Investing.Resources;

namespace Investing.TaxReport
{
    public class Application
    {
        private readonly BrokerReportParser _brokerReportParser;
        private readonly ExcelSheetService _excelSheetService;
        private readonly ExcelTradeSheetService _excelTradeSheetService;
        private readonly ExcelDividendSheetService _excelDividendSheetService;
        private readonly TradeService _tradeService;

        public Application(BrokerReportParser brokerReportParser,
            ExcelSheetService excelSheetService,
            ExcelTradeSheetService excelTradeSheetService,
            ExcelDividendSheetService excelDividendSheetService,
            TradeService tradeService)
        {
            _brokerReportParser = brokerReportParser;
            _excelSheetService = excelSheetService;
            _excelTradeSheetService = excelTradeSheetService;
            _excelDividendSheetService = excelDividendSheetService;
            _tradeService = tradeService;
        }

        public void Run()
        {
            var config = ConfigFactory.TestConfig;
            config.CurrentYear = 2022;
            config.Commission = 0;

            config.BrokerFiles
                .Select(fileName => _brokerReportParser.Parse(fileName))
                .ToList();
            
            var trades = _tradeService.GetAllTrades();

            var tradesByYear = GetTradesByYear(config.CurrentYear, trades);

            _excelSheetService.FileName = $"C:\\Reports\\{config.UserName}_Nalog_{config.CurrentYear}.xlsx";
            _excelTradeSheetService.Generate(config.CurrentYear, tradesByYear, config);
            _excelDividendSheetService.Generate(config.CurrentYear);
            _excelSheetService.Save();

                // var wordFile = $"{directory}\\Templates\\Note.docx";
                // var doc = new Document(wordFile);
                // doc.Range.Replace("%test%", "Hello World !!!", new FindReplaceOptions(FindReplaceDirection.Forward));
                // var wordOutputFile = $"D:\\Note_{_brokerInfo.CurrentYear}.doc";
                // doc.Save(wordOutputFile);
        }

        private static List<Trade> GetTradesByYear(int year, List<Trade> trades)
        {
            var result = new List<Trade>();

            foreach (var groupSymbol in trades.GroupBy(t => t.Symbol))
            {
                var tradesByYear = new List<Trade>();

                var list = groupSymbol.ToList();
                var start = list.FindLastIndex(t => t.Operation == Operation.Close && t.DateTime.Year < year);
                var end = list.FindLastIndex(t => t.Operation == Operation.Close && t.DateTime.Year < year + 1);

                for (int i = start + 1; i <= end; i++)
                {
                    tradesByYear.Add(list[i]);
                }

                result.AddRange(tradesByYear);
            }

            return result;
        }
    }
}