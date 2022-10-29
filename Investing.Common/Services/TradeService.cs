using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words.Fields;
using Investing.Common.Models;
using Investing.Common.Stores;

namespace Investing.Common.Services
{
    public class TradeService
    {
        private readonly TradeStore _tradeStore;
        private readonly CorporateActionStore _corporateActionStore;
        private readonly StockStore _stockStore;

        public TradeService(TradeStore tradeStore,
            CorporateActionStore corporateActionStore,
            StockStore stockStore)
        {
            _tradeStore = tradeStore;
            _corporateActionStore = corporateActionStore;
            _stockStore = stockStore;
        }

        private List<Trade> Separate(List<Trade> trades)
        {
            var queue = new Queue<KeyValue<decimal, Trade>>();

            var splits = _corporateActionStore.GetSplits();

            var openTrades = trades.Where(t => t.Operation == Operation.Open);

            foreach (var trade in openTrades)
            {
                var item = new KeyValue<decimal, Trade>(trade.QuantityAbs, trade);
                queue.Enqueue(item);
            }

            if (queue.Count == 0)
                return trades;

            var closeTrades = trades.Where(t => t.Operation == Operation.Close);

            var result = new List<Trade>();

            foreach (var closeTrade in closeTrades)
            {
                var splitFind =
                    splits.SingleOrDefault(s => s.Simbol == closeTrade.Symbol && closeTrade.DateTime >= s.DateTime);

                var quantity = closeTrade.QuantityAbs;

                while (quantity != 0)
                {
                    var que = queue.Peek();

                    if (splitFind != null)
                    {
                        var currentTrade = que.Value;
                        if (currentTrade.DateTime < splitFind.DateTime && !currentTrade.IsSplitted)
                        {
                            currentTrade.Quantity *= splitFind.To;
                            currentTrade.TransactionPrice /= splitFind.To;
                            currentTrade.IsSplitted = true;
                            que.Key *= splitFind.To;
                        }
                    }

                    if (quantity >= que.Key)
                    {
                        var it = queue.Dequeue();
                        if (it.Value.Quantity != it.Key)
                        {
                        }

                        it.Value.Quantity = it.Key;
                        quantity -= it.Key;
                        result.Add(it.Value);
                    }
                    else
                    {
                        var newTrade = que.Value.Clone();
                        newTrade.Ref = que.Value;
                        newTrade.Quantity = quantity;
                        newTrade.Comission = (que.Value.Comission / que.Value.Quantity) * quantity;
                        result.Add(newTrade);

                        que.Key -= quantity;
                        que.Value.Comission = (que.Value.Comission / que.Value.Quantity) * que.Key;
                        que.Value.Quantity = que.Key;
                        break;
                    }
                }

                result.Add(closeTrade);
            }

            if (queue.Count > 0)
            {
                var lastTrades = queue.Select(q => q.Value);
                result.AddRange(lastTrades);
            }

            return result;
        }

        public List<Trade> GetAllTrades()
        {
            var trades = _tradeStore.GetAll();

            var spinOffs = _corporateActionStore.GetSpinOffs();

            foreach (var spinOff in spinOffs)
            {
                var find = spinOff;

                var all = trades.Where(t => t.Symbol == find.FromSymbol).ToList();
                var trade = all[0];
                var quantity = GetQuantityStocks(all, find.DateTime);
                var numFrom = (int)(quantity / find.From);
                var numTo = numFrom * find.To;

                var newTrade = new Trade()
                {
                    Symbol = find.ToSymbol,
                    Quantity = numTo,
                    Comission = 0,
                    DateTime = find.DateTime,
                    Currency = trade.Currency,
                    ActiveType = trade.ActiveType
                };

                trades.Add(newTrade);
            }

            var result = new List<Trade>();

            foreach (var groupSymbol in trades.GroupBy(t => t.Symbol))
            {
                var tr = Separate(groupSymbol.ToList());
                result.AddRange(tr);
            }

            return result;
        }

        private decimal GetQuantityStocks(List<Trade> trades, DateTime dateTime)
        {
            decimal quantity = 0;

            foreach (var trade in trades)
            {
                if (trade.DateTime >= dateTime)
                    continue;
                if (trade.Operation == Operation.Open)
                {
                    quantity += trade.QuantityAbs;
                }
                else if (trade.Operation == Operation.Close)
                {
                    quantity -= trade.QuantityAbs;
                }
            }

            return quantity;
        }
    }
}