using System.Collections.Generic;
using System.Linq;
using Investing.Common.Models;

namespace Investing.Common.Stores
{
    public class StockStore
    {
        private readonly Dictionary<string, Dictionary<string, Stock>> _stocks =
            new Dictionary<string, Dictionary<string, Stock>>();

        public void Add(Stock stock)
        {
            var dict = new Dictionary<string, Stock>();

            if (_stocks.ContainsKey(stock.Exchange))
            {
                dict = _stocks[stock.Exchange];
            }
            else
            {
                _stocks.Add(stock.Exchange, dict);
            }

            if (!dict.ContainsKey(stock.Symbol))
            {
                dict.Add(stock.Symbol, stock);
            }
        }

        public Stock BySymbol(string symbol, string currency)
        {
            var list = new List<Stock>();

            foreach (var stocksValue in _stocks.Values)
            {
                if (stocksValue.ContainsKey(symbol))
                {
                    var stock = stocksValue[symbol];
                    list.Add(stock);
                }
            }

            if (list.Count >= 2)
                return list.SingleOrDefault(i => i.Currency == currency);

            return list.Count == 1 ? list[0] : null;
        }
    }
}