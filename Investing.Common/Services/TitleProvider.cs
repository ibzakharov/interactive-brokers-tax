using Investing.Common.Stores;

namespace Investing.Common.Services
{
    public class TitleProvider
    {
        private readonly StockStore _stockStore;

        public TitleProvider(StockStore stockStore)
        {
            _stockStore = stockStore;
        }

        public string GetTitleName(string symbol, string currency)
        {
            var stockName = "???";
            var stock = _stockStore.BySymbol(symbol, currency);
            if (stock != null)
            {
                stockName = stock.Name;
            } 
            return $"{symbol} - {stockName}    Валюта: {currency}";
        }
    }
}