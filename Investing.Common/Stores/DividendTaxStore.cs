using System;
using System.Collections.Generic;
using System.Linq;
using Investing.Common.Models;

namespace Investing.Common.Stores
{
    public class DividendTaxStore
    {
        private List<DividendTax> _list = new List<DividendTax>();

        public void Add(DividendTax item)
        {
            if (item.Sum > 0)
            {
                var find = _list.SingleOrDefault(i => i.Date == item.Date);
                if (find != null)
                {
                    if (find.Sum + item.Sum == 0)
                    {
                        _list.Remove(find);
                    }
                }

            }
            else
            {
                _list.Add(item);
            }
        }

        public DividendTax ByDateAndSymbol(DateTime dateTime, string symbol)
        {
            var list = _list.Where(d => d.Date == dateTime && d.Symbol == symbol)
                .ToList();
            if (list.Count > 1)
                Console.WriteLine($"По символу {symbol} обнаружено несколько значений");
            return list.Count == 0 ? null : list[0];
        }
    }
}