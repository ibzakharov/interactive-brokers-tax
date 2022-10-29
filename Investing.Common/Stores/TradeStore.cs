using System;
using System.Collections.Generic;
using Investing.Common.Models;

namespace Investing.Common.Stores
{
    public class TradeStore
    {
        private List<Trade> _list = new List<Trade>();

        public void Add(Trade item)
        {
            if (item.TransactionPrice == 0 && item.Comission == 0 && item.Gain == 0 && item.RealizedPL == 0)
            {
                Console.WriteLine($"Обнаружена непонятная сделка по символу {item.Symbol}");
            }
            else
            {
                _list.Add(item);
            }
        }

        public List<Trade> GetAll() => _list;
    }
}