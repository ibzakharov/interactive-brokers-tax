using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Investing.Common.Models;
using Investing.Common.Services;
using Investing.Resources;

namespace Investing.YahooFinance
{
    public class Application
    {
        private readonly BrokerReportParser _brokerReportParser;
        private readonly TradeService _tradeService;

        public Application(BrokerReportParser brokerReportParser,
            TradeService tradeService)
        {
            _brokerReportParser = brokerReportParser;
            _tradeService = tradeService;
        }

        public void Run()
        {
            var config = ConfigFactory.TestConfig;
            config.CurrentYear = 2022;
            var brokerFileInfos = config.BrokerFiles
                .Select(fileName => _brokerReportParser.Parse(fileName))
                .ToList();
            
            var trades = _tradeService.GetAllTrades();
            var groupBySymbol = trades.GroupBy(g => g.Symbol);

            var lst = new List<Trade>();
            foreach (var grouping in groupBySymbol)
            {
                var list = grouping.ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (item.Operation == Operation.Close)
                        break;
                    lst.Add(item);
                }
            }

            var builder = new StringBuilder();
            builder.AppendLine("Symbol,Trade Date,Purchase Price,Quantity,Commission,Comment");
            foreach (var tr in lst.OrderBy(i => i.DateTime))
            {
                if (tr.Symbol == "RDS B")
                {
                    tr.Symbol = "SHEL";
                }

                if (tr.Symbol == "LHAd")
                {
                    tr.Symbol = "LHA.DE";
                }

                if (tr.Symbol == "IMB")
                {
                    tr.Symbol = "IMB.L";
                }

                if (tr.Symbol == "TUI1d")
                {
                    tr.Symbol = "TUI1.DE";
                }

                if (tr.Symbol == "IAG")
                {
                    tr.Symbol = "IAG.L";
                }
                
                var date = $"{tr.DateTime.Year}{tr.DateTime.Month:00}{tr.DateTime.Day:00}";
                var ci = CultureInfo.InvariantCulture;
                var line =
                    $"{tr.Symbol},{date},{tr.TransactionPrice.ToString("0.0000", ci)}," +
                    $"{tr.QuantityAbs.ToString("0.0000", ci)},{tr.ComissionAbs.ToString("0.0000", ci)},";
                builder.AppendLine(line);
            }

            File.WriteAllText($"Z:\\{config.UserName}_Yahoo.csv", builder.ToString());
        }
    }
}