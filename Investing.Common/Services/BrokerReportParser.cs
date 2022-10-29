using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Investing.Common.Models;
using Investing.Common.Stores;
using Investing.Resources;

namespace Investing.Common.Services
{
    public class BrokerReportParser
    {
        private readonly StockStore _stockStore;
        private readonly DividendStore _dividendStore;
        private readonly DividendTaxStore _dividendTaxStore;
        private readonly TradeStore _tradeStore;
        private readonly SplitService _splitService;
        private readonly CorporateActionStore _corporateActionStore;

        public BrokerReportParser(StockStore stockStore,
            DividendStore dividendStore,
            DividendTaxStore dividendTaxStore,
            TradeStore tradeStore,
            SplitService splitService,
            CorporateActionStore corporateActionStore)
        {
            _stockStore = stockStore;
            _dividendStore = dividendStore;
            _dividendTaxStore = dividendTaxStore;
            _tradeStore = tradeStore;
            _splitService = splitService;
            _corporateActionStore = corporateActionStore;
        }

        public BrokerFileInfo Parse(BrokerFile brokerFile)
        {
            var brokerFileInfo = new BrokerFileInfo
            {
                Year = brokerFile.Year
            };

            var lines = brokerFile.FileText.Split('\r', '\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("Сделки,Data,Order,Акции,") ||
                    line.StartsWith("Сделки,Data,Order,Варранты,"))
                {
                    var trade = ParseTrade(line);
                    _tradeStore.Add(trade);
                }

                if (line.StartsWith("Дивиденды,Data,Всего"))
                {
                    ;
                }
                else if (line.StartsWith("Дивиденды,Data,"))
                {
                    var dividend = ParseDividend(line);
                    if (dividend != null)
                    {
                        _dividendStore.Add(dividend);
                    }
                }


                if (line.StartsWith("Удерживаемый налог,Data,Всего"))
                {
                }
                else if (line.StartsWith("Удерживаемый налог,Data,"))
                {
                    var dividendTax = ParseDividendTax(line);
                    if (dividendTax != null)
                    {
                        _dividendTaxStore.Add(dividendTax);
                    }
                }

                if (line.StartsWith("Информация о финансовом инструменте,Data,Акции,") ||
                    line.StartsWith("Информация о финансовом инструменте,Data,Варранты,"))
                {
                    var stock = ParseStock(line);
                    if (stock != null)
                    {
                        _stockStore.Add(stock);
                    }
                }

                if (line.StartsWith("Сборы/комиссии,Data,Всего,,,,"))
                {
                    brokerFileInfo.Commission = ParseCommission(line);
                }

                if (line.StartsWith("Корпоративные действия,Data,Акции"))
                {
                    if (line.Contains(" Сплит "))
                    {
                        var split = ParseStockSplit(line);
                        if (split != null)
                        {
                            _corporateActionStore.AddSplit(split);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка при обработки сплита");
                            Console.WriteLine(line);
                        }
                    }
                    else if (line.Contains(" Спин-офф "))
                    {
                        var spinOff = ParseSpinOff(line);
                        if (spinOff != null)
                        {
                            _corporateActionStore.AddSpinOff(spinOff);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка при обработки спин-офф");
                            Console.WriteLine(line);
                        }
                    }
                }
            }

            return brokerFileInfo;
        }

        private Trade ParseTrade(string line)
        {
            // var split = line.Split(",");

            var pattern = "(\\w|\\.|-|\\s)*,|\"(\\w|-|:|,|\\s)*\",";
            var test = Regex.Matches(line, pattern);
            var list = new List<string>();
            foreach (Match m in test)
            {
                var value = m.Value.TrimStart('\"').TrimEnd('\"', ',');
                list.Add(value);
            }

            var split = list.ToArray();


            var trade = new Trade();
            trade.ActiveType = split[3];
            trade.Currency = split[4];
            trade.Symbol = split[5];
            trade.DateTime = DateTime.ParseExact(split[6], "yyyy-MM-dd, HH:mm:ss", CultureInfo.InvariantCulture);
            // var day = DateTime.ParseExact(split[6].TrimStart('\"'), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            // var time = TimeSpan.ParseExact(split[7].Trim().TrimEnd('\"'), @"hh\:mm\:ss",
            //     CultureInfo.InvariantCulture);
            // trade.DateTime = day + time;
            trade.Quantity = Decimal.Parse(split[7], CultureInfo.InvariantCulture);
            trade.TransactionPrice = Decimal.Parse(split[8], CultureInfo.InvariantCulture);
            // trade.ClosePrice = Decimal.Parse(split[10], CultureInfo.InvariantCulture);
            trade.Gain = Decimal.Parse(split[10], CultureInfo.InvariantCulture);
            var comission = Decimal.Parse(split[11], CultureInfo.InvariantCulture);
            trade.Comission = comission; //Math.Round(comission, 2);
            // trade.Basis = Decimal.Parse(split[13], CultureInfo.InvariantCulture);
            trade.RealizedPL = Decimal.Parse(split[13], CultureInfo.InvariantCulture);
            // trade.MarketRevaluationPL = Decimal.Parse(split[15], CultureInfo.InvariantCulture);
            //trade.Code = split[15];
            return trade;
        }

        private Dividend ParseDividend(string line)
        {
            try
            {
                var split = line.Split(",");

                var dividend = new Dividend();
                dividend.Currency = split[2];
                var day = DateTime.ParseExact(split[3].TrimStart('\"'), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dividend.Date = day;
                var symbolStr = split[4];
                dividend.Symbol = symbolStr.Remove(symbolStr.IndexOf("(")).Trim();
                dividend.Sum = decimal.Parse(split[5], CultureInfo.InvariantCulture);
                return dividend;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private DividendTax ParseDividendTax(string line)
        {
            try
            {
                var split = line.Split(",");

                var dividendTax = new DividendTax();
                dividendTax.Currency = split[2];
                var day = DateTime.ParseExact(split[3].TrimStart('\"'), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dividendTax.Date = day;
                var symbolStr = split[4];
                dividendTax.Symbol = symbolStr.Remove(symbolStr.IndexOf("(")).Trim();
                dividendTax.Sum = decimal.Parse(split[5], CultureInfo.InvariantCulture);
                return dividendTax;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private Stock ParseStock(string line)
        {
            try
            {
                var split = line.Split(",");

                var stock = new Stock();
                stock.Symbol = split[3];
                stock.Name = split[4];
                stock.Identity = split[6];
                stock.Exchange = split[7];
                return stock;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        private decimal ParseCommission(string line)
        {
            var split = line.Split(",");
            return Decimal.Parse(split[6], CultureInfo.InvariantCulture);
        }

        private StockSplit ParseStockSplit(string line)
        {
            var split = _splitService.SplitLine(line);

            var dateTime = split[5];
            var description = split[6];
            var splitRegex = @"^(?<symbol>\w+)\(\w+\).+Сплит.+(?<to>\d+).+за.+(?<from>\d+).+\(.+\)$";
            var match = Regex.Match(description, splitRegex);

            if (match.Success)
            {
                var stockSplit = new StockSplit();
                stockSplit.Simbol = match.Groups["symbol"].Value;
                stockSplit.From = Convert.ToInt32(match.Groups["from"].Value);
                stockSplit.To = Convert.ToInt32(match.Groups["to"].Value);
                stockSplit.DateTime = Convert.ToDateTime(dateTime);
                return stockSplit;
            }

            return null;
        }

        private SpinOff ParseSpinOff(string line)
        {
            var split = _splitService.SplitLine(line);

            var dateTime = split[4];
            var description = split[6];
            var splitRegex =
                @"^(?<s_from>\w+)\(\w+\)\s+Спин-офф\s+(?<to>\d+)\s+за\s+(?<from>\d+)\s+\((?<s_to>\w+)\s+.*\)$";
            var match = Regex.Match(description, splitRegex);

            if (match.Success)
            {
                var stockSplit = new SpinOff();
                stockSplit.From = Convert.ToInt32(match.Groups["from"].Value);
                stockSplit.To = Convert.ToInt32(match.Groups["to"].Value);
                stockSplit.FromSymbol = match.Groups["s_from"].Value;
                stockSplit.ToSymbol = match.Groups["s_to"].Value;
                stockSplit.DateTime = Convert.ToDateTime(dateTime);
                return stockSplit;
            }

            return null;
        }
    }
}